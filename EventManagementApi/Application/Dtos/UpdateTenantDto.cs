namespace EventManagementApi.Application.Dtos
{
    public class UpdateTenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
