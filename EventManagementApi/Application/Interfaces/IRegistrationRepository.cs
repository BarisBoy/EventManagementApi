using EventManagementApi.Domain.Entities;
using EventManagementApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventManagementApi.Application.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<Registration?> GetByIdAsync(Guid id);
        Task<IEnumerable<Registration>> GetByEventIdAsync(Guid eventId);
        Task CreateAsync(Registration registration);
        Task UpdateAsync(Registration registration);
        Task DeleteAsync(Registration registration);
    }   
}