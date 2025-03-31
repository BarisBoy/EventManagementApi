using AutoMapper;
using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Infrastructure.Caching;
using EventManagementApi.Infrastructure.Helpers;
using EventManagementApi.Shared.Constants.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagementApi.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICacheService _cacheService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantService _tenantService;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly IJwtHelper _jwtHelper;
        private readonly IMapper _mapper;

        public AuthenticationService(ICacheService cacheService, IHttpContextAccessor httpContextAccessor, ITenantService tenantService, IJwtHelper jwtHelper, IMapper mapper, IUserRepository userRepository)
        {
            _cacheService = cacheService;
            _httpContextAccessor = httpContextAccessor;
            _tenantService = tenantService;
            _passwordHasher = new PasswordHasher<string>();
            _jwtHelper = jwtHelper;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<string> Login(LoginDto model)
        {
            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user.Email, user.PasswordHash, model.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Şifre yanlış.");
            }

            var token = _jwtHelper.GenerateJwtToken(user.Id.ToString(), user.TenantId.ToString(), user.Role);

            return token;
        }

        public async Task Register(CreateUserDto model)
        {
            // Tenant'ı doğrulamak için tenantId kontrolü
            var tenant = await _tenantService.GetTenantByIdAsync(model.TenantId);
            if (tenant == null)
            {
                throw new Exception("Geçerli bir tenant bulunamadı.");
            }
            // Email üzerinden User kontrolü
            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                throw new Exception("Bu e-posta adresiyle kayıtlı kullanıcı zaten mevcut.");
            }

            var userEntity = _mapper.Map<User>(model);
            // short olan role'un geçerli olup olmadığını kontrol et
            if (!Enum.IsDefined(typeof(Role), model.Role))
            {
                throw new Exception("Geçersiz rol tipi.");
            }
            // Password hashleme
            userEntity.PasswordHash = _passwordHasher.HashPassword(model.Email, model.Password);

            await _userRepository.CreateUserAsync(userEntity);
            var userDto = _mapper.Map<UserDto>(userEntity);
            await _cacheService.SetAsync($"user-{userDto.Id}", userDto);
        }

        public Guid? GetUserIdFromToken(HttpContext context)
        {
            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : (Guid?)null;
        }

        public Guid? GetTenantIdFromToken(HttpContext context)
        {
            var tenantIdClaim = context.User?.FindFirst("tenantId")?.Value;
            return tenantIdClaim != null ? Guid.Parse(tenantIdClaim) : (Guid?)null;
        }
    }
}
