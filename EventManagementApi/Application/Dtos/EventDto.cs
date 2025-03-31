namespace EventManagementApi.Application.Dtos
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public short Status { get; set; }

        public TenantDto Tenant { get; set; }
        public ICollection<RegistrationDto> Registrations { get; set; }
    }
}
