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
    public class AuthenticationServiceTests
    {
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ITenantService> _mockTenantService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IJwtHelper> _mockJwtHelper;

        private readonly IMapper _mapper;
        private readonly AuthenticationService _authenticationService;
        private readonly PasswordHasher<string> _passwordHasher;

        public AuthenticationServiceTests()
        {
            _mockCacheService = new Mock<ICacheService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockTenantService = new Mock<ITenantService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJwtHelper = new Mock<IJwtHelper>();
            _passwordHasher = new PasswordHasher<string>();
            
            // AutoMapper setup
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();  // Add your MappingProfile here
            });
            _mapper = configuration.CreateMapper(); // Create the mapper instance


            _authenticationService = new AuthenticationService(
                _mockCacheService.Object,
                _mockHttpContextAccessor.Object,
                _mockTenantService.Object,
                _mockJwtHelper.Object,
                _mapper,
                _mockUserRepository.Object
            );
        }

        [Fact]
        public async Task Login_ReturnsToken_WhenValidCredentials()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.Parse("AE49831E-A01A-4176-9F48-07805048EB0C"),
                UserName = "user1 upt",
                Email = "user1@tenant1.com",
                PasswordHash = "AQAAAAIAAYagAAAAEF2A3ALM3aWdHiAjh0lekldls1f0eFog4ePBxT3SZdc8EYrrbjqPt0T56B89XBffBA==",  // Bu hash, testte kullanılacak.
                Role = (short)Role.Admin,
                TenantId = Guid.Parse("4D0D91BB-4163-4299-813C-8F5AD56E7976")
            };

            var loginDto = new LoginDto
            {
                Email = "user1@tenant1.com",
                Password = "user1passs" // Bu şifre, hashedpassword ile eşleşmeli
            };

            // Mock'lar
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);
            _mockJwtHelper.Setup(jwt => jwt.GenerateJwtToken(user.Id.ToString(), user.TenantId.ToString(), user.Role)).Returns("dummyToken");

            // Act
            var token = await _authenticationService.Login(loginDto);

            // Assert
            Assert.Equal("dummyToken", token);
        }

        [Fact]
        public async Task Login_ThrowsUnauthorizedAccessException_WhenUserNotFound()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "user1@tenant5.com",
                Password = "password"
            };

            // Mock: Kullanıcı bulunamadığında null dönecek.
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authenticationService.Login(loginDto));
        }

        [Fact]
        public async Task Login_ThrowsUnauthorizedAccessException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user1@tenant1.com",
                PasswordHash = "AQAAAAIAAYagAAAAEF2A3ALM3aWdHiAjh0lekldls1f0eFog4ePBxT3SZdc8EYrrbjqPt0T56B89XBffBA==",
                Role = (short)Role.Admin,
                TenantId = Guid.NewGuid()
            };

            var loginDto = new LoginDto
            {
                Email = "user1@tenant1.com",
                Password = "wrongpassword" // Bu şifre, hashedpassword ile eşleşmeyecek.
            };

            // Mock'lar
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);
            _mockJwtHelper.Setup(jwt => jwt.GenerateJwtToken(user.Id.ToString(), user.TenantId.ToString(), user.Role)).Returns("dummyToken");

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authenticationService.Login(loginDto));
        }

        [Fact]
        public async Task Register_CreatesUser_WhenValidData()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                UserName = "johndoe",
                Name = "John",
                Surname = "Doe",
                Email = "newuser@example.com",
                Password = "password",
                Role = (short)Role.Admin,
                TenantId = Guid.Parse("4D0D91BB-4163-4299-813C-8F5AD56E7976")
            };

            var tenant = new Tenant { Id = createUserDto.TenantId };
            var userEntity = new User
            {
                Id = Guid.NewGuid(),
                UserName = createUserDto.UserName,
                Name = createUserDto.Name,
                Surname = createUserDto.Surname,
                Email = createUserDto.Email,
                PasswordHash = _passwordHasher.HashPassword(createUserDto.Email, createUserDto.Password),
                Role = createUserDto.Role,
                TenantId = createUserDto.TenantId
            };

            // Mock'lar
            var tenantDto = _mapper.Map<TenantDto>(tenant);
            _mockTenantService.Setup(service => service.GetTenantByIdAsync(createUserDto.TenantId)).ReturnsAsync(tenantDto);
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(createUserDto.Email)).ReturnsAsync((User)null);

            // Act
            await _authenticationService.Register(createUserDto);
            // Assert
            _mockUserRepository.Verify(repo => repo.CreateUserAsync(It.Is<User>(u =>
                u.Email == userEntity.Email &&
                //u.PasswordHash == userEntity.PasswordHash &&   // Password hashler farklı olabileceğinden kaldırıldı.
                u.Role == userEntity.Role &&
                u.TenantId == userEntity.TenantId &&
                u.UserName == userEntity.UserName &&  // Ensure all properties match
                u.Name == userEntity.Name &&
                u.Surname == userEntity.Surname
            )), Times.Once);
        }
    }

}
