using EventManagementApi.Domain.Entities;

namespace EventManagementApi.Application.Interfaces
{
    public interface ITenantRepository
    {
        Task<Tenant> GetTenantByIdAsync(Guid tenantId);
        Task<Tenant> CreateTenantAsync(Tenant tenant);
    }
}