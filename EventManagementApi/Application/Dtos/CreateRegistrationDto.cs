
namespace EventManagementApi.Application.Dtos
{
    public class CreateRegistrationDto
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string AttendeeName { get; set; }
        public string AttendeeEmail { get; set; }
        public short Status { get; set; }
    }
}