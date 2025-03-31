namespace EventManagementApi.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<User> Users { get; set; }
    }
}