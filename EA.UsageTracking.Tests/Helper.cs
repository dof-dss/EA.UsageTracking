using System;
using System.Collections.Generic;
using System.Text;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EA.UsageTracking.Tests
{
    public static class Helper
    {
        public static void PurgeTable<T>(this UsageTrackingContext usageTrackingContext, DbSet<T> table) where T : BaseEntity
        {
            foreach (var row in table)
            {
                usageTrackingContext.Set<T>().Remove(row);
            }
        }

        public static DbContextOptions<UsageTrackingContext> CreateNewContextOptionsUsingInMemoryDatabase()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<UsageTrackingContext>();
            builder.UseInMemoryDatabase("EA.UsageTracking")
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}
