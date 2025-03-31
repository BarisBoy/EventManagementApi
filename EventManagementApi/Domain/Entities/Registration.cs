
namespace EventManagementApi.Domain.Entities
{
    public class Registration : BaseEntity
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string AttendeeName { get; set; }
        public string AttendeeEmail { get; set; }
        public short Status { get; set; }

        public Event Event { get; set; }
        public User User { get; set; }
    }
}