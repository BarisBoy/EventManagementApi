using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventManagementApi.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        //private readonly ILogger<RedisCacheService> _logger;
        private readonly TimeSpan _defaultSlidingExpiration;
        private readonly TimeSpan _defaultAbsoluteExpiration;

        public RedisCacheService(IDistributedCache cache, IConfiguration configuration/*, ILogger<RedisCacheService> logger*/)
        {
            _cache = cache;
            //_logger = logger;

            if (!TimeSpan.TryParse(configuration["Redis:DefaultSlidingExpiration"], out _defaultSlidingExpiration))
            {
                _defaultSlidingExpiration = TimeSpan.FromHours(1); // Varsayılan 1 saat
                //_logger.LogWarning("Redis:DefaultSlidingExpiration yapılandırması hatalı. Varsayılan 30 dakika kullanılıyor.");
            }

            if (!TimeSpan.TryParse(configuration["Redis:DefaultAbsoluteExpiration"], out _defaultAbsoluteExpiration))
            {
                _defaultAbsoluteExpiration = TimeSpan.FromDays(1); // Varsayılan 1 gün
                //_logger.LogWarning("Redis:DefaultAbsoluteExpiration yapılandırması hatalı. Varsayılan 2 saat kullanılıyor.");
            }
        }

        public async Task SetAsync<T>(string key, T data, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = slidingExpiration ?? _defaultSlidingExpiration,
                    AbsoluteExpirationRelativeToNow = absoluteExpiration ?? _defaultAbsoluteExpiration
                };

                var serializedData = JsonConvert.SerializeObject(data);
                await _cache.SetStringAsync(key, serializedData, options);

                //_logger.LogInformation("Cache eklendi: {Key}", key);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Cache set işlemi başarısız: {Key}", key);
                throw;
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(key);
                if (cachedData is not null)
                {
                    //_logger.LogInformation("Cache bulundu: {Key}", key);
                    return JsonConvert.DeserializeObject<T>(cachedData);
                }

                //_logger.LogWarning("Cache bulunamadı: {Key}", key);
                return default;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Cache get işlemi başarısız: {Key}", key);
                return default;
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                //_logger.LogInformation("Cache silindi: {Key}", key);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Cache remove işlemi başarısız: {Key}", key);
                throw;
            }
        }
    }
}
