using EventManagementApi.Application.Dtos;
using EventManagementApi.Domain.Entities;

namespace EventManagementApi.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(Guid id);
        Task<List<Event>> GetAllAsync();
        Task<List<Event>> GetEventsByTenantAsync(Guid tenantId);
        Task AddAsync(Event eventEntity);
        Task UpdateAsync(Event eventEntity);
        Task DeleteAsync(Guid id);
        Task<EventStatisticsDto> GetEventStatisticsAsync(Guid eventId);
        Task<IEnumerable<Event?>> GetUpcomingEventsAsync();
    }
}
