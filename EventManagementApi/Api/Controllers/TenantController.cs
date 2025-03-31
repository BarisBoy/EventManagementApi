using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using EventManagementApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Api.Controllers
{
    [ApiController]
    [Route("api/tenants")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IAuthenticationService _authenticationService;

        public TenantController(ITenantService tenantService, IAuthenticationService authenticationService)
        {
            _tenantService = tenantService;
            _authenticationService = authenticationService;
        }

        // Yeni bir tenant oluşturma
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto tenantDto)
        {
            // Yeni tenant'ı oluştur
            var tenant = await _tenantService.CreateTenantAsync(tenantDto);
            return CreatedAtAction(nameof(GetCurrentTenant), new { id = tenant.Id }, tenant);
        }

        // Mevcut tenant bilgilerini alma
        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetCurrentTenant()
        {
            // Kullanıcının token'ından tenantId'yi al
            var tenantId = _authenticationService.GetTenantIdFromToken(HttpContext);

            if (tenantId == null)
            {
                return NotFound("Mevcut tenant bulunamadı.");
            }

            // Tenant bilgilerini al
            var tenant = await _tenantService.GetTenantByIdAsync(tenantId.Value);

            if (tenant == null)
            {
                return NotFound("Tenant bulunamadı.");
            }

            return Ok(tenant);
        }
    }
}
