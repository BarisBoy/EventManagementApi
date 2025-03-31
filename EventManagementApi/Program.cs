using EventManagementApi.Application.Interfaces;
using EventManagementApi.Application.Services;
using EventManagementApi.Domain.MultiTenancy;
using EventManagementApi.Infrastructure.Caching;
using EventManagementApi.Infrastructure.Filters;
using EventManagementApi.Infrastructure.Helpers;
using EventManagementApi.Infrastructure.Mapping;
using EventManagementApi.Infrastructure.Middleware;
using EventManagementApi.Infrastructure.Persistence;
using EventManagementApi.Infrastructure.Repositories;
using EventManagementApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DB Connection
builder.Services.AddDbContext<EventManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventManagementConnection")),
    ServiceLifetime.Scoped);

// Add JwtSecretKey
var jwtSecretKey = builder.Configuration.GetValue<string>("JwtSettings:SecretKey");
if (string.IsNullOrEmpty(jwtSecretKey))
{
    throw new Exception("JWT Secret Key is not configured properly.");
}

var key = Encoding.ASCII.GetBytes(jwtSecretKey);

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Eğer HTTPS kullanmıyorsanız, bu false olabilir.
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"], 
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])) // Anahtar doğru mu?
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Hata durumunda burada loglama yapılabilir.
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // Token doğrulandı, burada ek işlem yapabilirsiniz.
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// Add services to the container.

builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<IUserContextProvider, UserContextProvider>();

// appsettings.json'dan JWT ayarlarını okuma
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
//builder.Services.AddSingleton(jwtSecretKey);
builder.Services.AddSingleton<IJwtHelper, JwtHelper>(provider =>
{
    var jwtSettings = provider.GetRequiredService<IOptions<JwtSettings>>().Value;
    return new JwtHelper(jwtSettings.SecretKey, jwtSettings.Issuer, jwtSettings.Audience);
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IPasswordHasher<string>, PasswordHasher<string>>();

builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Swagger yapılandırması
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Event Management API", Version = "v1" });
    // JWT Authorization Bearer Token'ı tanımlama
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
    });

    // Global olarak header parametrelerini kullanma
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

// CORS(in case of need)
/* builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
}); */

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "EventManagementApi";
    //options.ConfigurationOptions.ConnectTimeout = 30000;
});
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// StatusCodePages middleware eklenmesiyle özelleştirilmiş hata mesajlarının döndürülmesi
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    var statusCode = response.StatusCode;
    var message = string.Empty;

    // Hata kodlarına göre özelleştirilmiş mesajlar
    switch (statusCode)
    {
        case 400:
            message = "Geçersiz istek. Lütfen parametrelerinizi kontrol edin.";
            break;
        case 401:
            message = "Kimlik doğrulama başarısız oldu. Lütfen tekrar giriş yapın.";
            break;
        case 403:
            message = "Bu işlemi yapmak için yetkiniz yok.";
            break;
        case 404:
            message = "Aradığınız kaynak bulunamadı.";
            break;
        case 405:
            message = "Desteklenmeyen HTTP metodu kullanıldı. Lütfen doğru metodu seçin.";
            break;
        case 409:
            message = "Çakışma hatası! Bu işlem gerçekleştirilemez.";
            break;
        case 429:
            message = "Çok fazla istek gönderildi. Lütfen bekleyin ve tekrar deneyin.";
            break;
        case 500:
            message = "Sunucu tarafında beklenmeyen bir hata oluştu. Lütfen tekrar deneyin.";
            break;
        case 502:
            message = "Geçersiz yanıt alındı. Sunucu bağlantısında sorun var.";
            break;
        case 503:
            message = "Sunucu şu an hizmet veremiyor. Lütfen daha sonra tekrar deneyin.";
            break;
        case 504:
            message = "Sunucu zaman aşımına uğradı. Lütfen daha sonra tekrar deneyin.";
            break;
        default:
            message = "Bilinmeyen bir hata oluştu. Lütfen sistem yöneticinizle iletişime geçin.";
            break;
    }

    // Özelleştirilmiş hata mesajı JSON formatında döndürme
    response.ContentType = "application/json";
    await response.WriteAsync(JsonConvert.SerializeObject(new
    {
        status = statusCode,
        error = Enum.GetName(typeof(HttpStatusCode), statusCode),
        message = message
    }));
});
// JWT Authentication ve Authorization
app.UseAuthentication();
app.UseAuthorization();

// Tenant Middleware
app.UseMiddleware<TenantMiddleware>();

// CORS (in case of need)
//app.UseCors("AllowAll");

app.MapControllers();

app.Run();
