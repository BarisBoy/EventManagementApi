using EventManagementApi.Application.Dtos;
using EventManagementApi.Domain.Entities;

namespace EventManagementApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto?> GetByIdAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetUsersByTenantAsync(Guid tenantId);
        Task CreateUserAsync(CreateUserDto user);
        Task<UserDto?> UpdateUserAsync(UpdateUserDto updateUser);
    }
}