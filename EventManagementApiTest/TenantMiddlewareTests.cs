
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using global::EventManagementApi.Domain.MultiTenancy;
using global::EventManagementApi.Infrastructure.Middleware;
using EventManagementApi.Infrastructure.Helpers;
using Castle.Core.Configuration;
using System.Net.Http;
using EventManagementApi.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using EventManagementApi.Shared.Constants.Enums;

namespace EventManagementApiTest
{

    public class TenantMiddlewareTests
    {
        private readonly Mock<IJwtHelper> _mockJwtHelper;
        private readonly Mock<ITenantProvider> _mockTenantProvider;
        private readonly TenantMiddleware _middleware;
        private readonly DefaultHttpContext _httpContext;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public TenantMiddlewareTests()
        {
            _mockJwtHelper = new Mock<IJwtHelper>();
            _mockTenantProvider = new Mock<ITenantProvider>();
            // Create in-memory configuration
            var inMemorySettings = new Dictionary<string, string>
            {
                {"JwtSettings:SecretKey", "EventManagementApiSecretKey202503"},
                {"JwtSettings:Issuer", "EventManagementApiIssuer"},
                {"JwtSettings:Audience", "EventManagementApiAudience"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _middleware = new TenantMiddleware(
                (innerHttpContext) => Task.CompletedTask,
            _mockJwtHelper.Object,
                _configuration);

            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task InvokeAsync_ValidJwtToken_ShouldSetTenantId()
        {
            // Arrange

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "AE49831E-A01A-4176-9F48-07805048EB0C"),
                new Claim("TenantId", "4D0D91BB-4163-4299-813C-8F5AD56E7976")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var _token = new JwtSecurityToken(
                issuer:  _configuration["JwtSettings:Issuer"],
                audience:  _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );
            var token = new JwtSecurityTokenHandler().WriteToken(_token);
            _httpContext.Request.Headers["Authorization"] = "Bearer " + token;

            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }, "TestAuthType", ClaimTypes.Name, ClaimTypes.Role));

            _mockJwtHelper.Setup(x => x.ValidateJwtToken(token)).Returns(_httpContext.User);

            // Act
            await _middleware.InvokeAsync(_httpContext, _mockTenantProvider.Object);

            // Assert
            _mockTenantProvider.Verify(x => x.SetTenantId( It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_NoTenantIdInRequest_ShouldReturnBadRequestOrUnauthorized()
        {
            // Arrange
            _httpContext.Request.Headers["Authorization"] = "Bearer invalid.token";

            // Act
            await _middleware.InvokeAsync(_httpContext, _mockTenantProvider.Object);

            // Assert
            var expectedStatusCodes = new List<int> { StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized };
            Assert.Contains(_httpContext.Response.StatusCode, expectedStatusCodes);
        }

        [Fact]
        public async Task InvokeAsync_ValidTenantIdInHeader_ShouldSetTenantId()
        {
            // Arrange
            _httpContext.Request.Headers["TenantId"] = "4D0D91BB-4163-4299-813C-8F5AD56E7976";
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }, "TestAuthType", ClaimTypes.Name, ClaimTypes.Role));
            // Act
            await _middleware.InvokeAsync(_httpContext, _mockTenantProvider.Object);

            // Assert
            Assert.Equal("4D0D91BB-4163-4299-813C-8F5AD56E7976", _httpContext.Items["TenantId"]);
        }

        [Fact]
        public async Task InvokeAsync_UnauthorizedWhenNotAuthenticated()
        {
            // Arrange
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            await _middleware.InvokeAsync(_httpContext, _mockTenantProvider.Object);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, _httpContext.Response.StatusCode);
        }
    }


}
