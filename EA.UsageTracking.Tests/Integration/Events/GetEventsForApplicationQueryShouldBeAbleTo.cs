using System.Threading.Tasks;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Events
{
    [TestFixture()]
    public class GetEventsForApplicationQueryShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task GetNoEvents()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetEventsForApplicationQuery {PageNumber = 1, PageSize = 100});

            //Assert
            Assert.AreEqual(0, results.Value.Total);
        }

        [Test]
        public async Task HandleGetEventsNoTenant()
        {
            //Act
            var results = await Mediator.Send(new GetEventsForApplicationQuery { PageNumber = 1, PageSize = 100 });

            //Assert
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, results.Error);
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
            Assert.AreEqual(3, results.Value.Total);
        }

        [Test]
        public async Task HandleGetEventsIncorrectPageNumber()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetEventsForApplicationQuery { PageNumber = 0, PageSize = 100 });

            //Assert
            Assert.AreEqual(Constants.ErrorMessages.InvalidPageNumber, results.Error);
        }

        [Test]
        public async Task HandleGetEventsIncorrectPageSize()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetEventsForApplicationQuery { PageNumber = 1, PageSize = 0 });

            //Assert
            Assert.AreEqual(Constants.ErrorMessages.InvalidPageSize, results.Error);
        }

        [Test]
        public async Task HandleGetEventsIncorrectPagination()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.ApplicationEvents.Add(new ApplicationEvent());
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetEventsForApplicationQuery { PageNumber = 0, PageSize = 0 });

            //Assert
            Assert.AreEqual($"{Constants.ErrorMessages.InvalidPageNumber},{Constants.ErrorMessages.InvalidPageSize}", results.Error);
        }
    }
}
