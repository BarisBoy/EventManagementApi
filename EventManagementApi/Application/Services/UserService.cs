using AutoMapper;
using EventManagementApi.Application.Dtos;
using EventManagementApi.Application.Interfaces;
using EventManagementApi.Domain.Entities;
using EventManagementApi.Infrastructure.Caching;
using EventManagementApi.Infrastructure.Repositories;
using EventManagementApi.Shared.Constants.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventManagementApi.Application.Services
{
    public class UserService : IUserService
    {
        public ICacheService _cacheService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly IMapper _mapper;
        public IAuthenticationService _authenticationService { get; set; }
        private readonly IUserRepository _userRepository;

        public UserService(ICacheService cacheService, IHttpContextAccessor httpContextAccessor, IPasswordHasher<string> passwordHasher, IMapper mapper, IAuthenticationService authenticactionService,  IUserRepository userRepository)
        {
            _cacheService = cacheService;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _authenticationService = authenticactionService;
            _userRepository = userRepository;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var cacheKey = $"user-by-email-{email}";
            var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey);
            if (cachedUser != null) return cachedUser;

            var user = await _userRepository.GetByEmailAsync(email);
            var userDto = _mapper.Map<UserDto>(user);
            if (userDto != null) await _cacheService.SetAsync(cacheKey, userDto);
            return userDto;
        }

        public async Task<UserDto?> GetByIdAsync(Guid userId)
        {
            var cacheKey = $"user-{userId}";
            var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey);
            if (cachedUser != null) return cachedUser;

            var user = await _userRepository.GetByIdAsync(userId);
            var userDto = _mapper.Map<UserDto>(user);
            if (userDto != null) await _cacheService.SetAsync(cacheKey, userDto);
            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByTenantAsync(Guid tenantId)
        {
            var cacheKey = $"users-by-tenant-{tenantId}";
            var cachedUsers = await _cacheService.GetAsync<IEnumerable<UserDto>>(cacheKey);
            if (cachedUsers != null) return cachedUsers;

            var users = await _userRepository.GetUsersByTenantAsync(tenantId);
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            await _cacheService.SetAsync(cacheKey, userDtos);
            return userDtos;
        }

        public async Task CreateUserAsync(CreateUserDto userDto)
        {
            var userEntity = _mapper.Map<User>(userDto);
            // short olan role'un geçerli olup olmadığını kontrol et
            if (!Enum.IsDefined(typeof(Role), userDto.Role))
            {
                throw new Exception("Geçersiz rol tipi.");
            }
            // Password hashleme
            userEntity.PasswordHash = _passwordHasher.HashPassword(userDto.Email, userDto.Password); 
            await _userRepository.CreateUserAsync(userEntity);
            var userDtoCache = _mapper.Map<UserDto>(userEntity);

            await ClearCache(userDtoCache.TenantId, userDtoCache.Id, userDtoCache.Email);
            var userCacheKey = $"user-{userDtoCache.Id}";
            await _cacheService.SetAsync(userCacheKey, userDtoCache);
        }

        // Kullanıcıyı güncelleme
        public async Task<UserDto?> UpdateUserAsync(UpdateUserDto updateUser)
        {
            var context = _httpContextAccessor.HttpContext;

            // Token'dan userId'nin alınması
            var userIdClaim = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = userIdClaim != null ? Guid.Parse(userIdClaim) : (Guid?)null;

            if (userId == null)
            {
                throw new Exception("Geçerli bir kullanıcı kimliği bulunamadı.");
            }

            // Token'dan tenantId'nin alınması
            var tenantIdClaim = context?.User?.FindFirst("tenantId")?.Value;
            var tenantId = tenantIdClaim != null ? Guid.Parse(tenantIdClaim) : (Guid?)null;

            // Kullanıcıyı id'sine göre bulunur
            var user = await _userRepository.GetByIdAsync(userId.Value);

            if (user == null)
            {
                return null; // Kullanıcı bulunamadıysa null döndürülür
            }

            // Kullanıcının tenantId'si ile gönderilen tenantId'yi kontrol edilir
            if (user.TenantId != tenantId)
            {
                throw new Exception("Bu işlem yalnızca kullanıcıya ait tenant için yapılabilir.");
            }
            // Kullanıcıyı güncelleme            
            user.UserName = updateUser.UserName; 
            user.Name = updateUser.Name;
            user.Surname = updateUser.Surname;
            user.Email = updateUser.Email;
            user.PasswordHash = _passwordHasher.HashPassword(updateUser.Email, updateUser.Password); // Password hashleme

            await _userRepository.UpdateUserAsync(user);
            var updatedUserDto = _mapper.Map<UserDto>(user);

            await ClearCache(tenantId.Value, userId.Value, user.Email);
            var userCacheKey = $"user-{updatedUserDto.Id}";
            await _cacheService.SetAsync(userCacheKey, updatedUserDto);

            return updatedUserDto;
        }
        private async Task ClearCache(Guid tenantId, Guid userId, string email)
        {
            // Cache temizleme işlemleri
            await _cacheService.RemoveAsync($"users-by-tenant-{tenantId}");
            await _cacheService.RemoveAsync($"user-{userId}");
            await _cacheService.RemoveAsync($"user-by-email-{email}");
        }
    }
}
