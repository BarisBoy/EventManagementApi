using EventManagementApi.Domain.MultiTenancy;

namespace EventManagementApi.Infrastructure.Helpers
{
    public class CacheKeyProvider
    {
        private readonly ITenantProvider _tenantProvider;

        public CacheKeyProvider(ITenantProvider tenantProvider)
        {
            _tenantProvider = tenantProvider;
        }

        public string GetCacheKey(string key)
        {
            var tenantId = _tenantProvider.GetTenantId();
            return $"{tenantId}-{key}"; // Tenant ID'yi önbellek anahtarına ekler
        }
    }
}
