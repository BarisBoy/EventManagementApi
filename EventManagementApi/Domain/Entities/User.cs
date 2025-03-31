
namespace EventManagementApi.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid TenantId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public short Role { get; set; }
        public Tenant Tenant { get; set; }
        public ICollection<Registration> Registrations { get; set; }
    }
}