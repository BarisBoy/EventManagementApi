using EventManagementApi.Shared.Constants.Enums;

namespace EventManagementApi.Application.Dtos
{
    public class RegisterDto
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid TenantId { get; set; }
        public Role Role { get; set; } = Role.Attendee; // Varsayılan olarak Attendee
    }
}