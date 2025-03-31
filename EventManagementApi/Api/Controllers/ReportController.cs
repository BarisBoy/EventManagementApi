using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using EventManagementApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Api.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IAuthenticationService _authenticationService;

        public ReportController(IEventService eventService, IAuthenticationService authenticationService)
        {
            _eventService = eventService;
            _authenticationService = authenticationService;
        }

        // Yaklaşan etkinlikler raporunu alma
        [HttpGet("upcoming-events")]
        [Authorize]
        public async Task<IActionResult> GetUpcomingEventsReport()
        {
            var tenantId = _authenticationService.GetTenantIdFromToken(HttpContext);
            if (tenantId == null)
            {
                return Unauthorized("Tenant bilgisi bulunamadı.");
            }

            var report = await _eventService.GetUpcomingEventsReportAsync();
            return Ok(report);
        }
    }
}
