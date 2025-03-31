using EventManagementApi.Application.Interfaces;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventManagementApi.Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly EventManagementDbContext _context;

        public TenantRepository(EventManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant> GetTenantByIdAsync(Guid tenantId)
        {
            return await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId && t.IsDeleted == false);
        }

        public async Task<Tenant> CreateTenantAsync(Tenant tenant)
        {
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
            return tenant;
        }
    }
}
