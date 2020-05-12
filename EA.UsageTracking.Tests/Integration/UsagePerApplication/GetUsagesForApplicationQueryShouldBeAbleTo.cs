using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.UsagePerApplication
{
    [TestFixture()]
    public class GetUsagesForApplicationQueryShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task HandleGetUsagesWithNoTenant()
        {
            //Act
            var results = await Mediator.Send(new GetUsagesForApplicationQuery { });

            //Assert
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, results.Error);
        }

        [Test]
        public async Task GetNoUsages()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsagesForApplicationQuery { });

            //Assert
            Assert.AreEqual(0, results.Value.Data.Count());
        }

        [Test]
        public async Task GetUsages()
        {
            //Arrange
            new SeedData(DbContext).PopulateTestData();

            //Act
            var results = await Mediator.Send(new GetUsagesForApplicationQuery { });

            //Assert
            Assert.AreEqual(2, results.Value.Data.Count());
        }

        [Test]
        public async Task HandleGetUsagesWithInvalidPageNumber()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsagesForApplicationQuery { PageNumber = 0});

            //Assert
            Assert.AreEqual(Constants.ErrorMessages.InvalidPageNumber, results.Error);
        }

        [Test]
        public async Task HandleGetUsagesWithInvalidPageSize()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsagesForApplicationQuery { PageSize = 0 });

            //Assert
            Assert.AreEqual(Constants.ErrorMessages.InvalidPageSize, results.Error);
        }

        [Test]
        public async Task HandleGetUsagesWithInvalidPagination()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsagesForApplicationQuery { PageNumber = 0, PageSize = 0 });

            //Assert
            Assert.AreEqual($"{Constants.ErrorMessages.InvalidPageNumber},{Constants.ErrorMessages.InvalidPageSize}", results.Error);
        }

    }
}
