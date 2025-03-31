namespace EventManagementApi.Application.Dtos
{
    public class EventStatisticsDto
    {
        public Guid EventId { get; set; }
        public int RegisteredCount { get; set; }
        public int AttendedCount { get; set; }
        // Other statistics properties...
    }
}