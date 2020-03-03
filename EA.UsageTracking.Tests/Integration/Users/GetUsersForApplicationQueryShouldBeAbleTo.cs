using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.Infrastructure.Features.Users.Queries;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace EA.UsageTracking.Tests.Integration.Users
{
    [TestFixture()]
    public class GetUsersForApplicationQueryShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task GetNoUsers()
        {
            //Act
            var results = await Mediator.Send(new GetUsersForApplicationQuery { PageNumber = 1, PageSize = 100 });

            //Assert
            Assert.AreEqual(0, results.Value.Count);
        }

        [Test]
        public async Task GetUsers()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.ApplicationUsers.Add(new ApplicationUser() {Id = Guid.NewGuid()});
            DbContext.ApplicationUsers.Add(new ApplicationUser() { Id = Guid.NewGuid() });
            DbContext.ApplicationUsers.Add(new ApplicationUser() { Id = Guid.NewGuid() });
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsersForApplicationQuery { PageNumber = 1, PageSize = 100 });

            //Assert
            Assert.AreEqual(3, results.Value.Count);
        }
    }
}
