using EventManagementApi.Infrastructure.Helpers;
using EventManagementApi.Domain.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using EventManagementApi.Infrastructure.Persistence;

namespace EventManagementApi.Infrastructure.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtHelper _jwtHelper;

        public TenantMiddleware(RequestDelegate next, IJwtHelper jwtHelper, IConfiguration configuration)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
            // JWT settings'i appsettings.json'dan dinamik olarak alınır
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");

            // JwtHelper'ı dinamik olarak yapılandırıyoruz
            _jwtHelper = new JwtHelper(secretKey, issuer, audience);
        }

        public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
        {

            var routeData = context.GetRouteData();
            var controllerName = routeData.Values["controller"]?.ToString();
            var actionName = routeData.Values["action"]?.ToString();

            // TenantId kontrolü bypass edilecek controller & action'lar
            var allowedEndpoints = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Authentication", new HashSet<string> { "Register" } }, // Register bypass edilecek
                { "Tenant", new HashSet<string> { "CreateTenant" } } // CreateTenant bypass edilecek (Hiçbir kullanıcı rolü uygun olmadığından Tenant üretim ortamında DB'de oluşturulmalı)
            };

            if (!string.IsNullOrEmpty(controllerName) && allowedEndpoints.TryGetValue(controllerName, out var allowedActions))
            {
                if (!string.IsNullOrEmpty(actionName) && allowedActions.Contains(actionName))
                {
                    if (context.User?.Identity == null || !context.User.Identity.IsAuthenticated)
                    {
                        await _next(context); // Tenant ID kontrolü yapılmadan devam edilir
                        return;
                    }
                    else
                    {
                        // İşlem için logout olunması gerekir
                        context.Response.StatusCode = StatusCodes.Status403Forbidden; // 403 Forbidden
                        await context.Response.WriteAsync("Forbidden: Please log out before performing this action.");
                        return;
                    }
                }
            }

            var loginEndpoints = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Authentication", new HashSet<string> { "Login" } } 
            };

            if (string.IsNullOrEmpty(controllerName) || !loginEndpoints.TryGetValue(controllerName, out var loginActions) || !loginActions.Contains(actionName))
            {
                // Kimlik doğrulama kontrolü
                if (context.User?.Identity == null || !context.User.Identity.IsAuthenticated)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized: Authentication required.");
                    return;
                }
            }

            var tenantId = context.Request.Headers["TenantId"].FirstOrDefault();

            if (string.IsNullOrEmpty(tenantId))
            {
                // Token'den tenantId alma işlemi
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token != null)
                {
                    var principal = _jwtHelper.ValidateJwtToken(token);

                    if (principal != null)
                    {
                        tenantId = principal.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value;
                    }
                }
            }

            if (string.IsNullOrEmpty(tenantId))
            {
                // TenantController spesifik controller için bypass edilir.
                if (!string.IsNullOrEmpty(controllerName) && controllerName.Equals("Tenant", StringComparison.OrdinalIgnoreCase))
                {
                    await _next(context);
                    return;
                }
                // Tenant ID bulunamazsa hata fırlatılabilir
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Tenant bilgisi bulunamadı.");
                return;
            }

            if (Guid.TryParse(tenantId, out var parsedTenantId))
            {
                tenantProvider.SetTenantId(parsedTenantId); // TenantId'nin setlenmesi
            }
            context.Items["TenantId"] = tenantId; // TenantId'nin requestte saklanması
            await _next(context);
        }
    }
}
