using System;
using System.Collections.Generic;
using System.Linq;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EA.UsageTracking.Application.API
{
    public static class SeedData
    {
        public static readonly Core.Entities.Application App = CreateApp();

        private static Core.Entities.Application CreateApp()
        {
            var app = new Core.Entities.Application()
            {
                Name = "Application 1",
                ApplicationEvents = new List<ApplicationEvent>()
                {
                    new ApplicationEvent() {Name = "Event 1"},
                    new ApplicationEvent() {Name = "Event 2"},
                    new ApplicationEvent() {Name = "Event 3"}
                }
            };

            app.UserToApplications = new List<UserToApplication>()
            {
                new UserToApplication
                {
                    ApplicationId = app.Id,
                    User = new ApplicationUser()
                    {
                        Id = Guid.Parse("b0ed668d-7ef2-4a23-a333-94ad278f4111"),
                        Name= "User 1",
                        Email = "User1@mail.com"
                    }
                },
                new UserToApplication
                {
                    ApplicationId = app.Id,
                    User = new ApplicationUser() {Id = Guid.NewGuid(), Name = "User 2", Email = "User2@mail.com"}
                },
                new UserToApplication
                {
                    ApplicationId = app.Id,
                    User = new ApplicationUser() {Id = Guid.NewGuid(), Name = "User 3", Email = "User3@mail.com"}
                }
            };

            return app;
        }

        public static readonly UsageItem UsageItem1 = new UsageItem
        {
            Application = App,
            ApplicationEvent = App.ApplicationEvents.First(),
            ApplicationUser = App.UserToApplications.First().User
        };

        public static readonly UsageItem UsageItem2 = new UsageItem
        {
            Application = App,
            ApplicationEvent = App.ApplicationEvents.First(),
            ApplicationUser = App.UserToApplications.Skip(1).First().User
        };

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var dbContext = new UsageTrackingContext(
                serviceProvider.GetRequiredService<DbContextOptions<UsageTrackingContext>>(), new Guid("b0ed668d-7ef2-4a23-a333-94ad278f45d7")))
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
            dbContext.Applications.Add(App);
            dbContext.UsageItems.Add(UsageItem1);
            dbContext.UsageItems.Add(UsageItem2);

            dbContext.SaveChanges();
        }
    }
}
