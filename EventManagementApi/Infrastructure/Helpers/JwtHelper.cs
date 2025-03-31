using EventManagementApi.Application.Dtos;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Shared.Constants.Enums;
using EventManagementApi.Shared.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventManagementApi.Infrastructure.Helpers
{
    public class JwtHelper : IJwtHelper
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtHelper(IOptions<JwtSettings> jwtSettings)
        {
            _secretKey = jwtSettings.Value.SecretKey ?? throw new ArgumentNullException(nameof(jwtSettings.Value.SecretKey));
            _issuer = jwtSettings.Value.Issuer ?? throw new ArgumentNullException(nameof(jwtSettings.Value.Issuer));
            _audience = jwtSettings.Value.Audience ?? throw new ArgumentNullException(nameof(jwtSettings.Value.Audience));
        }

        public JwtHelper(string secretKey, string ıssuer, string audience)
        {
            _secretKey = secretKey;
            _issuer = ıssuer;
            _audience = audience;
        }

        public string GenerateJwtToken(string userId, string tenantId, short role)
        {
            if (!Enum.IsDefined(typeof(Role), role))
            {
                throw new ArgumentException("Invalid role value.", nameof(role));
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("TenantId", tenantId),
                new Claim(ClaimTypes.Role, ((Role)Enum.ToObject(typeof(Role), role)).ToEnumString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetRoleFromToken(string token)
        {
            var principal = ValidateJwtToken(token);
            if (principal == null)
            {
                return null;
            }

            var roleClaim = principal.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value;
        }
    }
}
