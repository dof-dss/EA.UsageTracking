using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.Infrastructure.Features.Users.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace EA.UsageTracking.Tests.Integration.Users
{
    [TestFixture()]
    public class GetUsersForApplicationQueryShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task HandleNoApplication()
        {
            //Act
            var results = await Mediator.Send(new GetUsersForApplicationQuery { PageNumber = 1, PageSize = 100 });

            //Assert
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, results.Error);
        }

        [Test]
        public async Task GetNoUsers()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsersForApplicationQuery { PageNumber = 1, PageSize = 100 });

            //Assert
            Assert.AreEqual(0, results.Value.Data.Count());
        }

        [Test]
        public async Task GetUsers()
        {
            //Arrange
            var app = new Core.Entities.Application();
            app.UserToApplications = new List<UserToApplication>
                {
                    new UserToApplication{Application = app, User = new ApplicationUser() {Id = Guid.NewGuid()}},
                    new UserToApplication{Application = app, User = new ApplicationUser() {Id = Guid.NewGuid()}},
                    new UserToApplication{Application = app, User = new ApplicationUser() {Id = Guid.NewGuid()}},
                };
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsersForApplicationQuery { PageNumber = 1, PageSize = 100 });

            //Assert
            Assert.AreEqual("3", results.Value.Total);
        }

        [Test]
        public async Task GetUsersInMultiTenancy()
        {
            //Arrange
            var app = new Core.Entities.Application();
            app.UserToApplications = new List<UserToApplication>
            {
                new UserToApplication{Application = app, User = new ApplicationUser() {Id = Guid.NewGuid()}},
                new UserToApplication{Application = app, User = new ApplicationUser() {Id = Guid.NewGuid()}},
                new UserToApplication{Application = app, User = new ApplicationUser() {Id = Guid.NewGuid()}},
            };
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            DbContext.TenantId = "SomeRandomClientId";
            var app2 = new Core.Entities.Application();
            app2.UserToApplications = new List<UserToApplication>
            {
                new UserToApplication
                {
                    Application = app2, User = new ApplicationUser() {Id = Guid.NewGuid()}
                },
                new UserToApplication
                {
                    Application = app2, User = new ApplicationUser() {Id = Guid.NewGuid()}
                }
            };

            DbContext.Applications.Add(app2);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsersForApplicationQuery { PageNumber = 1, PageSize = 100 });

            //Assert
            Assert.AreEqual("3", results.Value.Total);
        }
    }
}
