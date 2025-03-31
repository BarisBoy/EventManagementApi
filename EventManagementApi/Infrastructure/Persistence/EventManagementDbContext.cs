using EventManagementApi.Domain.Entities;
using EventManagementApi.Domain.MultiTenancy;
using EventManagementApi.Infrastructure.Services;
using EventManagementApi.Shared.Constants.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventManagementApi.Infrastructure.Persistence
{
    public class EventManagementDbContext : DbContext
    {
        private readonly ITenantProvider _tenantProvider;
        private readonly IUserContextProvider _userContextProvider; // Kullanıcı bilgisi için context provider


        public EventManagementDbContext(DbContextOptions<EventManagementDbContext> options, ITenantProvider tenantProvider, IUserContextProvider userContextProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider;
            _userContextProvider = userContextProvider;
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }

        //public TenantQuery<User> UsersQuery => new(Users, _tenantProvider);
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            var currentUserId = _userContextProvider.GetCurrentUserId(); // Sistemdeki kullanıcıyı buradan al

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).SetAuditFields(currentUserId.ToString());
                }
                else if (entry.State == EntityState.Modified)
                {
                    ((BaseEntity)entry.Entity).UpdateAuditFields(currentUserId.ToString());
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var tenantId = _tenantProvider.GetTenantId();

            // Tenant bazlı filtreleme
            modelBuilder.Entity<Event>().HasQueryFilter(e => e.TenantId == tenantId);
            modelBuilder.Entity<Registration>().HasQueryFilter(r => r.Event.TenantId == tenantId);
            modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == tenantId);

            // Performans için Tenant indeksleri
            modelBuilder.Entity<Event>()
            .HasIndex(e => e.TenantId); 

            modelBuilder.Entity<User>()
                .HasIndex(u => u.TenantId);

            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.Domain) // Domain eşsiz olmalı
                .IsUnique();

            // Default Guid üretimi (NEWID() for SQL Server)
            modelBuilder.Entity<Event>()
                .Property(e => e.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Registration>()
                .Property(r => r.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Tenant>()
                .Property(t => t.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("NEWID()");

            // Soft delete uygulaması
            modelBuilder.Entity<Event>().Property(e => e.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Registration>().Property(a => a.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<User>().Property(u => u.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Tenant>().Property(t => t.IsDeleted).HasDefaultValue(false);

            // İlişkiler
            // OnDelete(DeleteBehavior.Restrict) Örn: Tenant silindiğinde Event silinmez
            // Tenant ve Event 
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Tenant)
                .WithMany(t => t.Events)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ve Tenant
            modelBuilder.Entity<User>()
                .HasOne(u => u.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event ve Attendee (Registration)
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ve Attendee (Registration)
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
