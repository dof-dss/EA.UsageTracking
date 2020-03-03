using System.Threading.Tasks;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Events
{
    [TestFixture()]
    public class GetEventsForApplicationQueryShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task GetNoEvents()
        {
            //Act
            var results = await Mediator.Send(new GetEventsForApplicationQuery {PageNumber = 1, PageSize = 100});

            //Assert
            Assert.AreEqual(0, results.Value.Count);
        }

        [Test]
        public async Task GetEvents()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetEventsForApplicationQuery { PageNumber = 1, PageSize = 100 });

            //Assert
            Assert.AreEqual(3, results.Value.Count);
        }
    }
}
