using EventManagementApi.Application.Dtos;
using static EventManagementApi.Api.Controllers.EventController;

namespace EventManagementApi.Application.Interfaces
{
    public interface IRegistrationService
    {
        Task<RegistrationDto> GetRegistrationAsync(Guid eventId, Guid id);
        Task<IEnumerable<RegistrationDto>> GetRegistrationsAsync(Guid eventId);
        Task<RegistrationDto> CreateRegistrationAsync(Guid eventId, CreateRegistrationDto model);
        Task<RegistrationDto> UpdateRegistrationAsync(Guid eventId, Guid id, UpdateRegistrationDto model);
        Task<RegistrationDto> DeleteRegistrationAsync(Guid eventId, Guid id);
    }
}