using AutoMapper;
using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace EventManagementApi.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;
        private readonly ICacheService _cacheService;

        public EventService(ICacheService cacheService, IMapper mapper, IEventRepository eventRepository)
        {
            _cacheService = cacheService;
            _mapper = mapper;
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {            
            string cacheKey = "events-all";
            var cachedEvents = await _cacheService.GetAsync<IEnumerable<EventDto>>(cacheKey);
            if (cachedEvents is not null) return cachedEvents;

            var events = await _eventRepository.GetAllAsync();
            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            await _cacheService.SetAsync(cacheKey, eventDtos);

            return eventDtos;
        }

        public async Task<IEnumerable<EventDto>> GetEventsByTenantAsync(Guid tenantId)
        {
            var cacheKey = $"events-by-tenant-{tenantId}";
            var cachedEvents = await _cacheService.GetAsync<IEnumerable<EventDto>>(cacheKey);
            if (cachedEvents is not null) return cachedEvents;

            var events = await _eventRepository.GetEventsByTenantAsync(tenantId);
            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            await _cacheService.SetAsync(cacheKey, eventDtos);

            return eventDtos;
        }

        public async Task<EventDto?> GetEventByIdAsync(Guid id)
        {
            string cacheKey = $"event-{id}";
            var cachedEvent = await _cacheService.GetAsync<EventDto>(cacheKey);
            if (cachedEvent is not null) return cachedEvent;

            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null) throw new Exception("Etkinlik bulunamadı.");
            
            var eventDto = _mapper.Map<EventDto>(eventEntity);
            await _cacheService.SetAsync(cacheKey, eventDto);

            return eventDto;
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto eventDto)
        {
            var eventEntity = _mapper.Map<Event>(eventDto);  
            await _eventRepository.AddAsync(eventEntity);
            var createdEventDto = _mapper.Map<EventDto>(eventEntity);

            await ClearCache(createdEventDto);
            string cacheKey = $"event-{createdEventDto.Id}";
            await _cacheService.SetAsync(cacheKey, createdEventDto);

            return createdEventDto;  
        }

        public async Task<EventDto> UpdateEventAsync(Guid userId, Guid eventId, UpdateEventDto eventDto)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
            {
                throw new Exception("Etkinlik bulunamadı.");
            }

            // Tenant kontrolü: etkinlik, geçerli tenant'a ait mi?
            if (eventEntity.TenantId != eventDto.TenantId)
            {
                throw new UnauthorizedAccessException("Bu etkinlik, geçerli tenant'a ait değil.");
            }

            // Silinmiş etkinlik üzerinde güncelleme yapılmaz, TODO: Silinmiş etkinliklerde güncelleme yapılabilir mi?
            if (eventEntity.IsDeleted)
            {
                throw new InvalidOperationException("Silinmiş etkinlik üzerinde güncelleme yapılamaz.");
            }

            eventEntity.Title = eventDto.Title;
            eventEntity.Description = eventDto.Description;
            eventEntity.StartDate = eventDto.StartDate;
            eventEntity.EndDate = eventDto.EndDate;
            eventEntity.Location = eventDto.Location;
            eventEntity.Capacity = eventDto.Capacity;
            eventEntity.Status = eventDto.Status;

            // Veritabanında etkinliği güncelle
            await _eventRepository.UpdateAsync(eventEntity);
            var updatedEventDto = _mapper.Map<EventDto>(eventEntity);

            await ClearCache(updatedEventDto);
            string cacheKey = $"event-{updatedEventDto.Id}";
            await _cacheService.SetAsync(cacheKey, updatedEventDto);
            return updatedEventDto;
        }

        // Etkinliği silmek (soft delete) için metot
        public async Task<EventDto> DeleteEventAsync(Guid userId, Guid tenantId, Guid eventId)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);

            // Tenant kontrolü: etkinlik, geçerli tenant'a ait mi?
            if (eventEntity?.TenantId != tenantId)
            {
                throw new UnauthorizedAccessException("Bu etkinlik, geçerli tenant'a ait değil.");
            }
            // Silinmiş etkinlik üzerinde güncelleme yapılmaz, TODO: Silinmiş etkinliklerde güncelleme yapılabilir mi?
            if (eventEntity.IsDeleted)
            {
                throw new InvalidOperationException("Silinmiş etkinlik üzerinde güncelleme yapılamaz.");
            }

            // Soft delete işlemi
            eventEntity.IsDeleted = true;

            // Veritabanında güncelleme yap
            await _eventRepository.UpdateAsync(eventEntity);
            var deletedEventDto = _mapper.Map<EventDto>(eventEntity);
 
            await ClearCache(deletedEventDto);
            string cacheKey = $"event-{deletedEventDto.Id}";
            await _cacheService.SetAsync(cacheKey, deletedEventDto);
            return _mapper.Map<EventDto>(deletedEventDto);
        }

        public async Task<EventStatisticsDto> GetEventStatisticsAsync(Guid eventId)
        {
            var statistics = await _eventRepository.GetEventStatisticsAsync(eventId);
            return _mapper.Map<EventStatisticsDto>(statistics);
        }

        public async Task<IEnumerable<EventDto>> GetUpcomingEventsReportAsync()
        {
            var upcomingEvents = await _eventRepository.GetUpcomingEventsAsync();
            return _mapper.Map<IEnumerable<EventDto>>(upcomingEvents);
        }
        private async Task ClearCache(EventDto eventDto)
        {
            // Cache Temizleme
            await _cacheService.RemoveAsync($"events-all");
            await _cacheService.RemoveAsync($"events-by-tenant-{eventDto.TenantId}");
            await _cacheService.RemoveAsync($"event-{eventDto.Id}");
        }
    }
}
