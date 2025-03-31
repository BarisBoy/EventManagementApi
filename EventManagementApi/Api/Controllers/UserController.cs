using AutoMapper;
using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        public UserController(IAuthenticationService authenticationService, IUserService userService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
        }

        // Tenant'a ait kullanıcılar listesi
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            // Token'dan tenant ID'yi al
            var tenantId = _authenticationService.GetTenantIdFromToken(HttpContext);

            if (tenantId == null)
            {
                return Unauthorized("Geçerli bir tenant kimliği bulunamadı.");
            }

            // Tenant ID'ye göre kullanıcıları al
            var users = await _userService.GetUsersByTenantAsync(tenantId.Value);

            if (users == null || !users.Any())
            {
                return NotFound("Bu tenant için kullanıcı bulunamadı.");
            }

            return Ok(users);
        }

        // Kullanıcı profilini almak (Mevcut kullanıcı bilgisi)
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            // Token'dan kullanıcı ID'yi al
            var userId = _authenticationService.GetUserIdFromToken(HttpContext);

            if (userId == null)
            {
                return Unauthorized("Geçerli bir kullanıcı kimliği bulunamadı.");
            }

            // Token'dan tenant ID'yi al
            var tenantId = _authenticationService.GetTenantIdFromToken(HttpContext);

            // Kullanıcıyı ve tenant'ı almak
            var user = await _userService.GetByIdAsync(userId.Value);

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            return Ok(user);
        }

        // Kullanıcı profilini güncellemek
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfileAsync([FromBody] UpdateUserDto userUpdateDto)
        {
            // Kullanıcıyı güncellemek
            var updatedUser = await _userService.UpdateUserAsync(userUpdateDto);

            if (updatedUser == null)
            {
                return NotFound("Kullanıcı güncellenemedi.");
            }

            return Ok(updatedUser);
        }
    }
}