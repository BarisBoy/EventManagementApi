namespace EventManagementApi.Domain.MultiTenancy
{
    public interface ITenantProvider
    {
        Guid GetTenantId();
        void SetTenantId(Guid tenantId);
    }
}
