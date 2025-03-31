using EventManagementApi.Domain.Entities;

namespace EventManagementApi.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetUsersByTenantAsync(Guid tenantId);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}