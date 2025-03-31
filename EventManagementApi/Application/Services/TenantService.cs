using AutoMapper;
using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using EventManagementApi.Domain.Entities;

namespace EventManagementApi.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly IMapper _mapper;
        private readonly ITenantRepository _tenantRepository;

        public TenantService(IMapper mapper, ITenantRepository tenantRepository)
        {
            _mapper = mapper;
            _tenantRepository = tenantRepository;
        }

        public async Task<TenantDto?> GetTenantByIdAsync(Guid tenantId)
        {
            var tenant = await _tenantRepository.GetTenantByIdAsync(tenantId);
            return tenant == null ? null : _mapper.Map<TenantDto>(tenant);
        }

        public async Task<TenantDto> CreateTenantAsync(CreateTenantDto tenantDto)
        {
            var tenant = _mapper.Map<Tenant>(tenantDto);
            var createdTenant = await _tenantRepository.CreateTenantAsync(tenant);
            return _mapper.Map<TenantDto>(createdTenant);
        }

        public Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
