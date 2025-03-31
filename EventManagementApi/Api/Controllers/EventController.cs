using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IEventService _eventService;
        IRegistrationService _registrationService;

        public EventController(IAuthenticationService authenticationService, IEventService eventService, IRegistrationService registrationService)
        {
            _authenticationService = authenticationService;
            _eventService = eventService;
            _registrationService = registrationService;
        }

        // Etkinliklerin listelenmesi (TODO: filtreleme ile)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetEvents()
        {
            var tenantId = _authenticationService.GetTenantIdFromToken(HttpContext);
            var events = await _eventService.GetEventsByTenantAsync(tenantId.Value);
            return Ok(events);
        }

        // Etkinlik oluştur
        [HttpPost]
        [Authorize(Roles = "EventManager, Admin")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto model)
        {
            var tenantId = _authenticationService.GetTenantIdFromToken(HttpContext);
            model.TenantId = tenantId.Value;
            var eventCreated = await _eventService.CreateEventAsync(model);
            return CreatedAtAction(nameof(GetEventById), new { id = eventCreated.Id }, eventCreated);
        }

        // Etkinlik detaylarını alma
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            var eventDetails = await _eventService.GetEventByIdAsync(id);
            return Ok(eventDetails);
        }

        // Etkinlik güncelleme
        [HttpPut("{id}")]
        [Authorize(Roles = "EventManager, Admin")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventDto model)
        {
            // Token'dan tenant ID alınması
            var tenantId = _authenticationService.GetTenantIdFromToken(HttpContext);
            model.TenantId = tenantId.Value;
            // Token'dan kullanıcı ID alınması
            var userId = _authenticationService.GetUserIdFromToken(HttpContext);

            if (userId == null)
            {
                return Unauthorized("Geçerli bir kullanıcı kimliği bulunamadı.");
            }
            var updatedEvent = await _eventService.UpdateEventAsync(userId.Value, id, model);
            if (updatedEvent == null)
            {
                return NotFound("Etkinlik güncellenemedi.");
            }

            return Ok(updatedEvent);
        }

        // Etkinlik silme
        [HttpDelete("{id}")]
        [Authorize(Roles = "EventManager, Admin")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var userId = _authenticationService.GetUserIdFromToken(HttpContext);

            if (userId == null)
            {
                return Unauthorized("Geçerli bir kullanıcı kimliği bulunamadı.");
            }

            var eventDetails = await _eventService.GetEventByIdAsync(id);
            var tenantId = _authenticationService.GetTenantIdFromToken(HttpContext);

            var deleted = await _eventService.DeleteEventAsync(userId.Value, tenantId.Value, id);
            if (deleted == null)
            {
                return NotFound("Etkinlik silinemedi.");
            }

            return NoContent();
        }

        [HttpGet("{eventId}/registrations")]
        public async Task<IActionResult> GetRegistrations(Guid eventId)
        {
            var registrations = await _registrationService.GetRegistrationsAsync(eventId);
            return Ok(registrations);
        }

        [HttpPost("{eventId}/registrations")]
        [Authorize(Roles = "Attendee")]
        public async Task<IActionResult> CreateRegistration(Guid eventId, [FromBody] CreateRegistrationDto model)
        {
            var userId = _authenticationService.GetUserIdFromToken(HttpContext);

            if (userId == null)
            {
                return Unauthorized("Geçerli bir kullanıcı kimliği bulunamadı.");
            }

            var eventDetails = await _eventService.GetEventByIdAsync(eventId);
            model.UserId = userId.Value;
            model.EventId = eventId;
            var registration = await _registrationService.CreateRegistrationAsync(eventId, model);

            //return CreatedAtAction(nameof(GetRegistrationById), new { eventId, id = registration.Id }, registration);
            //return Ok(new { Message = "Kayıt işlemi başarıyla gerçekleştirildi. registrationId: " + registration.Id + "" });
            return Ok(registration);
        }

        [HttpPut("{eventId}/registrations/{id}")]
        [Authorize(Roles = "EventManager, Admin")]
        public async Task<IActionResult> UpdateRegistration(Guid eventId, Guid id, [FromBody] UpdateRegistrationDto model)
        {
            var registration = await _registrationService.UpdateRegistrationAsync(eventId, id, model);
            return Ok(registration);
        }

        [HttpDelete("{eventId}/registrations/{id}")]
        [Authorize(Roles = "EventManager, Admin")]
        public async Task<IActionResult> DeleteRegistration(Guid eventId, Guid id)
        {
            var deleted = await _registrationService.DeleteRegistrationAsync(eventId, id);
            if (deleted == null)
            {
                return NotFound("Kayıt silinemedi.");
            }
            //return NoContent();
            return Ok();
        }

        // Katılım istatistiklerini alma
        [HttpGet("events/{eventId}/statistics")]
        [Authorize(Roles = "EventManager, Admin")]
        public async Task<IActionResult> GetEventStatistics(Guid eventId)
        {
            var tenantId = _authenticationService.GetTenantIdFromToken(HttpContext);
            if (tenantId == null)
            {
                return Unauthorized("Tenant bilgisi bulunamadı.");
            }

            var statistics = await _eventService.GetEventStatisticsAsync(eventId);
            return Ok(statistics);
        }
    }
}
