using EventManagementApi.Shared.Constants.Enums;

namespace EventManagementApi.Application.Dtos
{
    public class RegistrationDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string AttendeeName { get; set; }
        public string AttendeeEmail { get; set; }
        public short Status { get; set; }

        public EventDto Event { get; set; }
        public UserDto User { get; set; }
    }
}