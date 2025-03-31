using Moq;
using EventManagementApi.Application.Services;
using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Infrastructure.Helpers;
using EventManagementApi.Shared.Constants.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using EventManagementApi.Infrastructure.Mapping;
using EventManagementApi.Infrastructure.Caching;
using Microsoft.AspNetCore.Identity;

namespace EventManagementApiTest
{
    public class EventServiceTests
    {
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IEventRepository> _mockEventRepository;
        private readonly IMapper _mapper;
        private readonly IEventService _eventService;

        public EventServiceTests()
        {
            _mockCacheService = new Mock<ICacheService>();
            _mockEventRepository = new Mock<IEventRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Event, EventDto>();
                cfg.CreateMap<CreateEventDto, Event>();
                cfg.CreateMap<UpdateEventDto, Event>();
            });
            _mapper = mapperConfig.CreateMapper();

            _eventService = new EventService(_mockCacheService.Object, _mapper, _mockEventRepository.Object);
        }

        [Fact]
        public async Task GetAllEventsAsync_ReturnsCachedEvents_WhenCacheExists()
        {
            // Arrange
            var cachedEvents = new List<EventDto>
            {
                new EventDto { Id = Guid.NewGuid(), Title = "Event 1" },
                new EventDto { Id = Guid.NewGuid(), Title = "Event 2" }
            };

            _mockCacheService.Setup(service => service.GetAsync<IEnumerable<EventDto>>("events-all"))
                             .ReturnsAsync(cachedEvents);

            // Act
            var result = await _eventService.GetAllEventsAsync();

            // Assert
            Assert.Equal(cachedEvents.Count, result.Count());
            _mockCacheService.Verify(service => service.GetAsync<IEnumerable<EventDto>>("events-all"), Times.Once);
            _mockEventRepository.Verify(repo => repo.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllEventsAsync_ReturnsEvents_WhenCacheMiss()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = Guid.NewGuid(), Title = "Event 1", TenantId = Guid.Parse("4D0D91BB-4163-4299-813C-8F5AD56E7976"), Status = (short)EventStatus.Scheduled, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Location", Capacity = 100},
                new Event { Id = Guid.NewGuid(), Title = "Event 2", TenantId = Guid.Parse("4D0D91BB-4163-4299-813C-8F5AD56E7976"), Status = (short)EventStatus.Scheduled, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Location = "Location", Capacity = 100},
            };
            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            _mockCacheService.Setup(service => service.GetAsync<IEnumerable<EventDto>>("events-all"))
                             .ReturnsAsync((IEnumerable<EventDto>)null);
            _mockEventRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(events);
            _mockCacheService.Setup(service => service.SetAsync("events-all", eventDtos, It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()));

            // Act
            var result = await _eventService.GetAllEventsAsync();

            // Assert
            Assert.Equal(eventDtos.Count(), result.Count());
            _mockCacheService.Verify(service => service.GetAsync<IEnumerable<EventDto>>("events-all"), Times.Once);
            _mockEventRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mockCacheService.Verify(service => service.SetAsync("events-all", It.IsAny<IEnumerable<EventDto>>(), It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Fact]
        public async Task GetEventByIdAsync_ReturnsEvent_WhenCacheMiss()
        {
            // Arrange
            var eventEntity = new Event { Id = Guid.NewGuid(), Title = "Event 1" };
            var eventDto = _mapper.Map<EventDto>(eventEntity);

            _mockCacheService.Setup(service => service.GetAsync<EventDto>("event-" + eventEntity.Id))
                             .ReturnsAsync((EventDto)null);
            _mockEventRepository.Setup(repo => repo.GetByIdAsync(eventEntity.Id)).ReturnsAsync(eventEntity);
            _mockCacheService.Setup(service => service.SetAsync("event-" + eventDto.Id, eventDto, It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()));

            // Act
            var result = await _eventService.GetEventByIdAsync(eventEntity.Id);

            // Assert
            Assert.Equal(eventDto.Title, result.Title);
            _mockCacheService.Verify(service => service.GetAsync<EventDto>("event-" + eventEntity.Id), Times.Once);
            _mockEventRepository.Verify(repo => repo.GetByIdAsync(eventEntity.Id), Times.Once);
            _mockCacheService.Verify(service => service.SetAsync("event-" + eventDto.Id, It.IsAny<EventDto>(), It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Fact]
        public async Task CreateEventAsync_CreatesEventAndCachesIt()
        {
            // Arrange
            var createEventDto = new CreateEventDto
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.Parse("4D0D91BB-4163-4299-813C-8F5AD56E7976"),
                Title = "New Event",
                Description = "Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Location = "Location",
                Capacity = 100,
                Status = (short)EventStatus.Scheduled,

            };
            var eventEntity = _mapper.Map<Event>(createEventDto);
            var eventDto = _mapper.Map<EventDto>(eventEntity);

            _mockEventRepository.Setup(repo => repo.AddAsync(eventEntity));
            _mockCacheService.Setup(service => service.SetAsync("event-" + eventDto.Id, eventDto, It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>())).Verifiable();

            // Act
            var result = await _eventService.CreateEventAsync(createEventDto);

            // Assert
            Assert.Equal(eventDto.Title, result.Title);
            _mockEventRepository.Verify(repo => repo.AddAsync(It.Is<Event>(u => 
            u.Title == eventEntity.Title 
            && u.Description == eventEntity.Description 
            && u.StartDate == eventEntity.StartDate 
            && u.EndDate == eventEntity.EndDate
            && u.Location == eventEntity.Location
            && u.Capacity == eventEntity.Capacity
            && u.Status == eventEntity.Status
            && u.TenantId == eventEntity.TenantId)), Times.Once);
            _mockCacheService.Verify(service => service.SetAsync("event-" + eventDto.Id, It.IsAny<EventDto>(), It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>()), Times.Once);
            _mockCacheService.Verify(service => service.RemoveAsync("events-all"), Times.Once); // Cache cleanup
        }

        [Fact]
        public async Task DeleteEventAsync_DeletesEventAndCachesIt()
        {
            // Arrange
            var eventEntity = new Event { Id = Guid.NewGuid(), Title = "Event to Delete", TenantId = Guid.NewGuid() };
            var eventDto = _mapper.Map<EventDto>(eventEntity);

            _mockEventRepository.Setup(repo => repo.GetByIdAsync(eventEntity.Id)).ReturnsAsync(eventEntity);
            _mockEventRepository.Setup(repo => repo.UpdateAsync(eventEntity)); // Soft delete

            // Act
            var result = await _eventService.DeleteEventAsync(Guid.NewGuid(), eventEntity.TenantId, eventEntity.Id);

            // Assert
            Assert.Equal(eventDto.Title, result.Title);
            _mockEventRepository.Verify(repo => repo.UpdateAsync(eventEntity), Times.Once);
            _mockCacheService.Verify(service => service.RemoveAsync("events-all"), Times.Once);
            _mockCacheService.Verify(service => service.RemoveAsync("event-" + eventDto.Id), Times.Once);
        }
    }

}
