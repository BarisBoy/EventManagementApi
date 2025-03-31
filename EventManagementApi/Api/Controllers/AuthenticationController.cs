using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly Application.Interfaces.IAuthenticationService _authenticationService;

        public AuthenticationController(Application.Interfaces.IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                var token = await _authenticationService.Login(model);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        //[HttpPost("logout")]
        //public IActionResult Logout()
        //{
        //    try
        //    {
        //        var userId = User.Identity?.Name;
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            return Unauthorized(new { Message = "Kullanıcı bulunamadı." });
        //        }

        //        _authenticationService.Logout(userId);
        //        return Ok(new { Message = "Logout başarılı." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Error = ex.Message });
        //    }
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto model)
        {
            try
            {
                await _authenticationService.Register(model);
                return Ok(new { Message = "Kullanıcı başarıyla kaydedildi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}