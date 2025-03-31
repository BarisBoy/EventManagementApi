using AutoMapper;
using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Infrastructure.Caching;
using EventManagementApi.Shared.Constants.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace EventManagementApi.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IMapper _mapper;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ICacheService _cacheService;

        public RegistrationService(ICacheService cacheService, IMapper mapper, IRegistrationRepository registrationRepository, IEventRepository eventRepository)
        {
            _cacheService = cacheService;
            _mapper = mapper;
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
        }


        public async Task<RegistrationDto> GetRegistrationAsync(Guid eventId, Guid id)
        {
            var cacheKey = $"registration-{eventId}-{id}";
            var cachedRegistration = await _cacheService.GetAsync<string>(cacheKey);
            if (cachedRegistration != null)
            {
                return JsonConvert.DeserializeObject<RegistrationDto>(cachedRegistration);
            }

            var registration = await _registrationRepository.GetByIdAsync(id);
            if (registration == null || registration.EventId != eventId)
            {
                return null;
            }
            var registrationDto = _mapper.Map<RegistrationDto>(registration);
            await _cacheService.SetAsync(cacheKey, JsonConvert.SerializeObject(registrationDto));
            return registrationDto;
        }

        public async Task<IEnumerable<RegistrationDto>> GetRegistrationsAsync(Guid eventId)
        {
            var cacheKey = $"registrations-for-event-{eventId}";
            var cachedRegistrations = await _cacheService.GetAsync<string>(cacheKey); 
            if (cachedRegistrations != null)
            {
                return JsonConvert.DeserializeObject<IEnumerable<RegistrationDto>>(cachedRegistrations);
            }

            var registrations = await _registrationRepository.GetByEventIdAsync(eventId);
            var registrationDtos = _mapper.Map<IEnumerable<RegistrationDto>>(registrations);
            await _cacheService.SetAsync(cacheKey, JsonConvert.SerializeObject(registrationDtos));
            return registrationDtos;
        }

        public async Task<RegistrationDto> CreateRegistrationAsync(Guid eventId, CreateRegistrationDto model)
        {
            var @event = await _eventRepository.GetByIdAsync(eventId);
            if (@event == null)
            {
                throw new InvalidOperationException("Etkinlik bulunamadı.");
            }

            var registration = new Registration
            {
                EventId = eventId,
                UserId = model.UserId,
                AttendeeName = model.AttendeeName,
                AttendeeEmail = model.AttendeeEmail,
                Status = (short)RegistrationStatus.Waitlisted

            };

            await _registrationRepository.CreateAsync(registration);
            var registrationDto = _mapper.Map<RegistrationDto>(registration);

            await ClearCache(eventId, registrationDto.Id);
            var cacheKey = $"registration-{eventId}-{registrationDto.Id}";
            await _cacheService.SetAsync(cacheKey, registrationDto);

            return registrationDto;
        }

        public async Task<RegistrationDto> UpdateRegistrationAsync(Guid eventId, Guid id, UpdateRegistrationDto model)
        {
            var registration = await _registrationRepository.GetByIdAsync(id);
            if (registration == null || registration.EventId != eventId)
            {
                return null;
            }

            // DTO'dan veriyi mevcut nesneye manuel olarak yansıtıyoruz
            registration.AttendeeName = model.AttendeeName;
            registration.AttendeeEmail = model.AttendeeEmail;
            registration.Status = model.Status;

            await _registrationRepository.UpdateAsync(registration);
            var registrationDto = _mapper.Map<RegistrationDto>(registration);

            await ClearCache(registration.EventId, registrationDto.Id);
            var cacheKey = $"registration-{registration.EventId}-{registrationDto.Id}";
            await _cacheService.SetAsync(cacheKey, registrationDto);

            return registrationDto;
        }

        public async Task<RegistrationDto> DeleteRegistrationAsync(Guid eventId, Guid id)
        {
            var registration = await _registrationRepository.GetByIdAsync(id);
            if (registration == null || registration.EventId != eventId)
            {
                return null;
            }

            registration.IsDeleted = true;
            await _registrationRepository.DeleteAsync(registration);
            var registrationDto = _mapper.Map<RegistrationDto>(registration);

            await ClearCache(registration.EventId, registrationDto.Id);
            var cacheKey = $"registration-{registration.EventId}-{registrationDto.Id}";
            await _cacheService.SetAsync(cacheKey, registrationDto);

            return registrationDto;
        }

        private async Task ClearCache(Guid eventId, Guid registrationId)
        {
            // Cache temizleme işlemleri
            await _cacheService.RemoveAsync($"registrations-by-event-{eventId}");
            await _cacheService.RemoveAsync($"registration-{eventId}");
            await _cacheService.RemoveAsync($"registration-{eventId}-{registrationId}");
        }
    }
}
