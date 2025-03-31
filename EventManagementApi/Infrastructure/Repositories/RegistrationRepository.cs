using EventManagementApi.Application.Interfaces;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventManagementApi.Infrastructure.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly EventManagementDbContext _context;

        public RegistrationRepository(EventManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Registration?> GetByIdAsync(Guid id)
        {
            return await _context.Registrations.FirstOrDefaultAsync(r => r.Id == id && r.IsDeleted == false);
        }

        public async Task<IEnumerable<Registration>> GetByEventIdAsync(Guid eventId)
        {
            return await _context.Registrations.Where(r => r.EventId == eventId && r.IsDeleted == false).ToListAsync();
        }

        public async Task CreateAsync(Registration registration)
        {
            await _context.Registrations.AddAsync(registration);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Registration registration)
        {
            _context.Registrations.Update(registration);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Registration registration)
        {
            _context.Registrations.Update(registration);
            await _context.SaveChangesAsync();
        }
    }
}
