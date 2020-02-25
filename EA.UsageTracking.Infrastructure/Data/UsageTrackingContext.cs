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

        public UsageTrackingContext(DbContextOptions<UsageTrackingContext> options)
            : base(options)
        {}

        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationEvent> ApplicationEvents { get; set; }
        public DbSet<UsageItem> UsageItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyAllConfigurationsFromCurrentAssembly();
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                                e.State == EntityState.Added
                                || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).DateModified = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).DateCreated = DateTime.Now;
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