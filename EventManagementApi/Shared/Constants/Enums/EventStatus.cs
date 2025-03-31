namespace EventManagementApi.Shared.Constants.Enums
{
    public enum EventStatus : short
    {
        Scheduled = 0,   // Planlanan
        Ongoing = 1,     // Devam Ediyor
        Completed = 2,   // Tamamlanmış
        Canceled = 3     // İptal Edilmiş
    }
}
