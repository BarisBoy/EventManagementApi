using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Infrastructure.Persistence;
using EventManagementApi.Shared.Constants.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventManagementApi.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventManagementDbContext _context;

        public EventRepository(EventManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted == false);
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events.ToListAsync();
        }
        public async Task<List<Event>> GetEventsByTenantAsync(Guid tenantId)
        {
            return await _context.Events.Where(e => e.TenantId == tenantId && e.IsDeleted == false).ToListAsync();
        }
        
        public async Task AddAsync(Event eventEntity)
        {
            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event eventEntity)
        {
            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Events.FindAsync(id);
            if (entity != null)
            {
                _context.Events.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<EventStatisticsDto> GetEventStatisticsAsync(Guid eventId)
        {
            var @event = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.IsDeleted == false && e.Id == eventId);

            if (@event == null)
            {
                return null;
            }

            var registeredCount = @event.Registrations.Count;
            var attendedCount = @event.Registrations.Count(r => r.Status == (short)RegistrationStatus.Approved && r.IsDeleted ==  false);

            return new EventStatisticsDto
            {
                EventId = @event.Id,
                RegisteredCount = registeredCount,
                AttendedCount = attendedCount
            };
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            var upcomingEvents = await _context.Events
                .Where(e => e.IsDeleted == false && e.StartDate >= DateTime.Now)
                .OrderBy(e => e.StartDate)
                .ToListAsync();
            return upcomingEvents;
        }
    }
}
