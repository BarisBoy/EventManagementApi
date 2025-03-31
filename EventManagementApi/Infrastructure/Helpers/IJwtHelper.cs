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
    public interface IJwtHelper
    {
        string GenerateJwtToken(string userId, string tenantId, short role);
        ClaimsPrincipal ValidateJwtToken(string token);
        string GetRoleFromToken(string token);
    }
}
