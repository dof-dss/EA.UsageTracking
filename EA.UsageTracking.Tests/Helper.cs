﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EA.UsageTracking.Tests
{
    public static class Helper
    {
        public static void PurgeTable<T>(this UsageTrackingContext usageTrackingContext, DbSet<T> table) where T : class
        {
            foreach (var row in table)
            {
                usageTrackingContext.Set<T>().Remove(row);
            }
            usageTrackingContext.SaveChanges();
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

        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                     | BindingFlags.Static;
            var field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

    }
}
