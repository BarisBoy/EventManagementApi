using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using EventManagementApi.Domain.MultiTenancy;
using EventManagementApi.Infrastructure.Services;

namespace EventManagementApi.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EventManagementDbContext>
    {
        public EventManagementDbContext CreateDbContext(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // appsettings.json'dan bağlantı dizesini alın
            var connectionString = config.GetConnectionString("EventManagementConnection");

            var optionsBuilder = new DbContextOptionsBuilder<EventManagementDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // 🔹 Dummy TenantProvider for design-time usage
            var tenantProvider = new DesignTimeTenantProvider();

            // 🔹 Dummy IUserContextProvider for design-time usage
            var userContextProvider = new DesignTimeUserContextProvider();

            return new EventManagementDbContext(optionsBuilder.Options, tenantProvider, userContextProvider);
        }
    }
    
    // 🔹 Dummy implementation for design-time usage
    public class DesignTimeTenantProvider : ITenantProvider
    {
        private Guid _tenantId = Guid.Parse("00000000-0000-0000-0000-000000000000"); // Default Tenant ID

        public Guid GetTenantId()
        {
            return _tenantId;
        }

        public void SetTenantId(Guid tenantId)
        {
            _tenantId = tenantId;
        }
    }

    // 🔹 Dummy implementation for design-time usage of IUserContextProvider
    public class DesignTimeUserContextProvider : IUserContextProvider
    {
        public Guid GetCurrentUserId()
        {
            // Dummy user ID for design-time
            return Guid.NewGuid();
        }

        public string GetCurrentUserName()
        {
            // Dummy user name for design-time
            return "DesignTimeUser";
        }
    }
}