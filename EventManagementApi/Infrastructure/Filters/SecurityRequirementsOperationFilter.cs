using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EventManagementApi.Infrastructure.Filters
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Login, Register ve TenantController'da Bearer Token kontrolünü kaldırıyoruz
            var excludedEndpoints = new[] { "Login", "Register", "CreateTenant" };

            // Eğer action adı, login, register veya tenant içeriyorsa Bearer token eklenmesin
            if (!excludedEndpoints.Any(endpoint => context.ApiDescription.ActionDescriptor.DisplayName.Contains(endpoint, StringComparison.InvariantCultureIgnoreCase)))
            {
                // Authorization (Bearer) header'ı her endpoint'e eklenmesi
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        { new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"  // Swagger'daki "Bearer" şeması ile eşleşmeli
                                }
                            }, new string[] {} }
                    }
                };
            }

            // Sadece "Login" action'ı için TenantId header'ının eklenmesi
            if (context.ApiDescription.ActionDescriptor.DisplayName.Contains("login", StringComparison.InvariantCultureIgnoreCase))
            //if (string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Post.Method, StringComparison.InvariantCultureIgnoreCase))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "TenantId",
                    In = ParameterLocation.Header,
                    Required = true,  // Burada opsiyonel yapmak isterseniz false yapabilirsiniz
                    Example = new OpenApiString("your-tenant-id-value")  // Örnek tenantId değeri
                });
            }
        }
    }
}
