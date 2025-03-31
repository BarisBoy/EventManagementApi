using EventManagementApi.Application.Dtos;
using EventManagementApi.Domain.Entities;

namespace EventManagementApi.Infrastructure.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expirySliding = null, TimeSpan? expiryAbsolute = null);
        Task RemoveAsync(string key);
    }

}