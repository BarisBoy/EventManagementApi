using EventManagementApi.Application.Dtos;

namespace EventManagementApi.Application.Interfaces
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantDto>> GetAllTenantsAsync();
        Task<TenantDto?> GetTenantByIdAsync(Guid tenantId);
        Task<TenantDto> CreateTenantAsync(CreateTenantDto tenantDto);
        //Task<bool> UpdateTenantAsync(Guid tenantId, UpdateTenantDto tenantDto);
        //Task<bool> DeleteTenantAsync(Guid tenantId);
    }
}
