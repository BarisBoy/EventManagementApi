namespace EventManagementApi.Application.Dtos
{
    public class CreateUserDto
    {
        public Guid TenantId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public short Role { get; set; }
    }
}