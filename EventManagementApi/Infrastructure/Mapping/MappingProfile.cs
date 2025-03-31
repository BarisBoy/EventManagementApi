using AutoMapper;
using EventManagementApi.Application.Dtos;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Shared.Constants.Enums;

namespace EventManagementApi.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Tenant Mappings
            CreateMap<CreateTenantDto, Tenant>();
            CreateMap<Tenant, TenantDto>().ReverseMap();

            // Event Mappings
            CreateMap<CreateEventDto, Event>();
            CreateMap<UpdateEventDto, Event>();
            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.Tenant, opt => opt.MapFrom(src => src.Tenant)) // Tenant'ı include et
                .ReverseMap();

            // User Mappings
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Tenant, opt => opt.MapFrom(src => src.Tenant)) // Tenant'ı include et
                .ReverseMap();

            // Registration (Attendee) Mappings
            CreateMap<CreateRegistrationDto, Registration>();
            CreateMap<UpdateRegistrationDto, Registration>();
            CreateMap<Registration, RegistrationDto>()
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event)) // Event'i include et
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))   // User'ı include et
                .ReverseMap();
        }
    }
}
