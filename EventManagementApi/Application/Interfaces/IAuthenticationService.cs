using EventManagementApi.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagementApi.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> Login(LoginDto model);
        Task Register(CreateUserDto model); 
        Guid? GetUserIdFromToken(HttpContext context);
        Guid? GetTenantIdFromToken(HttpContext context);
    }

}
