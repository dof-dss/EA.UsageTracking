using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EA.UsageTracking.API
{
    public static class SeedData
    {
        public static readonly Application App = new Application()
        {
            Name = "Application 1",
            ApplicationEvents = new List<ApplicationEvent>()
            {
                new ApplicationEvent() { Name = "Event 1" },
                new ApplicationEvent() { Name = "Event 2" },
                new ApplicationEvent() { Name = "Event 3" }
            },
            ApplicationUsers = new List<ApplicationUser>()
            {
                new ApplicationUser(){Name = "User 1", Email = "User1@mail.com"},
                new ApplicationUser(){Name = "User 2", Email = "User2@mail.com"},
                new ApplicationUser(){Name = "User 3", Email = "User3@mail.com"}
            }
        };

        public static readonly UsageItem UsageItem1 = new UsageItem
        {
            Application = App,
            ApplicationEvent = App.ApplicationEvents.First(),
            ApplicationUser = App.ApplicationUsers.First()
        };

        public static readonly UsageItem UsageItem2 = new UsageItem
        {
            Application = App,
            ApplicationEvent = App.ApplicationEvents.First(),
            ApplicationUser = App.ApplicationUsers.Skip(1).First()
        };

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var dbContext = new UsageTrackingContext(
                serviceProvider.GetRequiredService<DbContextOptions<UsageTrackingContext>>()))
            {
                // Look for any TODO items.
                if (dbContext.UsageItems.Any())
                {
                    return;   // DB has been seeded
                }

                PopulateTestData(dbContext);
            }
        }
        public static void PopulateTestData(UsageTrackingContext dbContext)
        {
            foreach (var item in dbContext.UsageItems)
            {
                dbContext.Remove(item);
            }
            dbContext.SaveChanges();
            dbContext.Applications.Add(App);
            dbContext.UsageItems.Add(UsageItem1);
            dbContext.UsageItems.Add(UsageItem2);

            dbContext.SaveChanges();
        }
    }
}
