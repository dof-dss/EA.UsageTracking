using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.EFCore.Extensions;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace EA.UsageTracking.Infrastructure.Data
{
    public class UsageTrackingContext : DbContext
    {
        private readonly Guid _tenantId;

        public UsageTrackingContext(DbContextOptions<UsageTrackingContext> options, Guid tenantId)
            : base(options)
        {
            _tenantId = tenantId;
        }

        public UsageTrackingContext(DbContextOptions<UsageTrackingContext> options)
            : base(options)
        {

        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationEvent> ApplicationEvents { get; set; }
        public DbSet<UsageItem> UsageItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationEvent>().Property<bool>("isDeleted");
            modelBuilder.Entity<Application>().Property<bool>("isDeleted");
            modelBuilder.Entity<ApplicationUser>().Property<bool>("isDeleted");
            modelBuilder.Entity<UsageItem>().Property<bool>("isDeleted");

            modelBuilder.Entity<Application>().Property<Guid>("_tenantId").HasColumnName("TenantId");
            modelBuilder.Entity<Application>()
                .HasQueryFilter(b => EF.Property<Guid>(b, "_tenantId") == _tenantId 
                                     && EF.Property<bool>(b, "isDeleted") == false);

            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(b => EF.Property<bool>(b, "isDeleted") == false);

            modelBuilder.Entity<ApplicationEvent>().Property<Guid>("_tenantId").HasColumnName("TenantId");
            modelBuilder.Entity<ApplicationEvent>().HasQueryFilter(b => EF.Property<Guid>(b, "_tenantId") == _tenantId
                                                                        && EF.Property<bool>(b, "isDeleted") == false);

            modelBuilder.Entity<UsageItem>().Property<Guid>("_tenantId").HasColumnName("TenantId");
            modelBuilder.Entity<UsageItem>().HasQueryFilter(b => EF.Property<Guid>(b, "_tenantId") == _tenantId 
                                                                 && EF.Property<bool>(b, "isDeleted") == false);

            modelBuilder.Entity<UserToApplication>().HasKey(ua => new { ua.UserId, ua.ApplicationId });
            modelBuilder.Entity<UserToApplication>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.UserToApplications)
                .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<UserToApplication>()
                .HasOne(pt => pt.Application)
                .WithMany(t => t.UserToApplications)
                .HasForeignKey(pt => pt.ApplicationId);

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyAllConfigurationsFromCurrentAssembly();
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => (e.Entity is IBaseEntity) && (
                                e.State == EntityState.Added
                                || e.State == EntityState.Modified
                                || e.State == EntityState.Deleted));

            foreach (var entityEntry in entries)
            {
                ((IBaseEntity)entityEntry.Entity).DateModified = DateTime.Now;

                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        ((IBaseEntity)entityEntry.Entity).DateCreated = DateTime.Now;

                        if (entityEntry.Metadata.GetProperties().Any(p => p.Name == "_tenantId"))
                            entityEntry.CurrentValues["_tenantId"] = _tenantId;

                        entityEntry.CurrentValues["isDeleted"] = false;
                        break;
                    case EntityState.Deleted:
                        entityEntry.State = EntityState.Modified;
                        entityEntry.CurrentValues["isDeleted"] = true;
                        break;
                }

            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}