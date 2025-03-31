namespace EventManagementApi.Domain.MultiTenancy
{
    public class TenantProvider : ITenantProvider
    {
        private static AsyncLocal<Guid> _tenantId = new();

        public Guid GetTenantId()
        {
            return _tenantId.Value;
        }

        public void SetTenantId(Guid tenantId)
        {
            _tenantId.Value = tenantId;
        }
    }
}
