
namespace EventManagementApi.Domain.Entities
{
    public class Event : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public short Status { get; set; }
        public Guid TenantId { get; set; }  
        public Tenant Tenant { get; set; }
        public ICollection<Registration> Registrations { get; set; }
    }
}