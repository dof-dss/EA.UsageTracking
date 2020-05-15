using System;
using System.Collections.Generic;
using System.Linq;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;

namespace EA.UsageTracking.Application.API
{
    public class SeedData
    {
        public readonly Core.Entities.Application App;

        public SeedData(UsageTrackingContext usageTrackingContext)
        {
            _usageTrackingContext = usageTrackingContext;
            App = CreateApp();
            _usageTrackingContext.Applications.Add(App);
            _usageTrackingContext.SaveChanges();
        }

        private Core.Entities.Application CreateApp()
        {
            var app = new Core.Entities.Application
            {
                Name = "Application 1",
                ApplicationEvents = new List<ApplicationEvent>
                {
                    new ApplicationEvent {Name = "Event 1"},
                    new ApplicationEvent {Name = "Event 2"},
                    new ApplicationEvent {Name = "Event 3"}
                }
            };

            app.UserToApplications = new List<UserToApplication>
            {
                new UserToApplication
                {
                    ApplicationId = app.Id,
                    User = new ApplicationUser
                    {
                        Id = Guid.Parse("b0ed668d-7ef2-4a23-a333-94ad278f4111"),
                        Name = "User 1",
                        Email = "User1@mail.com"
                    }
                },
                new UserToApplication
                {
                    ApplicationId = app.Id,
                    User = new ApplicationUser {Id = Guid.NewGuid(), Name = "User 2", Email = "User2@mail.com"}
                },
                new UserToApplication
                {
                    ApplicationId = app.Id,
                    User = new ApplicationUser {Id = Guid.NewGuid(), Name = "User 3", Email = "User3@mail.com"}
                }
            };

            return app;
        }

        private readonly UsageTrackingContext _usageTrackingContext;

        public void PopulateTestData()
        {
            var usageItem1 = new UsageItem
            {
                Application = App,
                ApplicationEvent = App.ApplicationEvents.First(),
                ApplicationUser = App.UserToApplications.First().User
            };

            var usageItem2 = new UsageItem
            {
                Application = App,
                ApplicationEvent = App.ApplicationEvents.First(),
                ApplicationUser = App.UserToApplications.Skip(1).First().User
            };

            _usageTrackingContext.UsageItems.Add(usageItem1);
            _usageTrackingContext.UsageItems.Add(usageItem2);
            _usageTrackingContext.SaveChanges();

        }
    }
}