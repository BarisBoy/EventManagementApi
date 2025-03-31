
namespace EventManagementApi.Application.Dtos
{
    public class TenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
        public ICollection<EventDto> Events { get; set; }
        public ICollection<UserDto> Users { get; set; }
    }
}
