
namespace EventManagementApi.Infrastructure.Services
{
    public interface IUserContextProvider
    {
        Guid GetCurrentUserId();  // Sistemdeki geçerli kullanıcıyı döndürür
        public string GetCurrentUserName();
    } 
}