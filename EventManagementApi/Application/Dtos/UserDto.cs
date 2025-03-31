namespace EventManagementApi.Application.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        //public string PasswordHash { get; set; }
        public Guid TenantId { get; set; }
        public short Role { get; set; }
        public TenantDto Tenant { get; set; }
        public ICollection<RegistrationDto> Registrations { get; set; }
    }
}