using EventManagementApi.Application.Dtos;
using EventManagementApi.Domain.Entities;

namespace EventManagementApi.Application.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<IEnumerable<EventDto>> GetEventsByTenantAsync(Guid tenantId);
        Task<EventDto?> GetEventByIdAsync(Guid eventId);
        Task<EventDto> CreateEventAsync(CreateEventDto eventDto);
        Task<EventDto> UpdateEventAsync(Guid userId, Guid tenantId, UpdateEventDto eventDto);
        Task<EventDto> DeleteEventAsync(Guid userId, Guid tenantId, Guid eventId);
        Task<EventStatisticsDto> GetEventStatisticsAsync(Guid eventId);
        Task<IEnumerable<EventDto>> GetUpcomingEventsReportAsync();
    }
}