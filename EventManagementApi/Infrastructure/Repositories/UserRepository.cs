using EventManagementApi.Application.Interfaces;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventManagementApi.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EventManagementDbContext _context;

        public UserRepository(EventManagementDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.Where(u => u.IsDeleted == false && u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users.Where(u => u.Id == userId && u.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByTenantAsync(Guid tenantId)
        {
            return await _context.Users.Where(u => u.TenantId == tenantId && u.IsDeleted == false).ToListAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"User creation failed: {ex.Message}", ex);
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
