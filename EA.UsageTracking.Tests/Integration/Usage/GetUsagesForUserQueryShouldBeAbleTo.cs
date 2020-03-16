using System;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Features.Usages.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Usage
{
    [TestFixture]
    public class GetUsagesForUserQueryShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task GetUsagesForUser()
        {
            //Arrange
            new SeedData(DbContext).PopulateTestData();

            //Act
            var results = await Mediator.Send(new GetUsagesForUserQuery {Id = Guid.Parse("b0ed668d-7ef2-4a23-a333-94ad278f4111") });

            //Assert
            Assert.AreEqual(1, results.Value.Total);
        }

        [Test]
        public async Task GetNoUsagesForUser()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsagesForUserQuery { Id = Guid.NewGuid() });

            //Assert
            Assert.AreEqual(0, results.Value.Total);
        }

        [Test]
        public async Task HandleGetUsagesWithNoTenant()
        {
            //Act
            var results = await Mediator.Send(new GetUsagesForUserQuery { });

            //Assert
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, results.Error);
        }

        [Test]
        public async Task HandleGetUsagesWithInvalidPageNumber()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsagesForUserQuery { PageNumber = 0 });

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
            var results = await Mediator.Send(new GetUsagesForUserQuery { PageSize = 0 });

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
            var results = await Mediator.Send(new GetUsagesForUserQuery { PageNumber = 0, PageSize = 0 });

            //Assert
            Assert.AreEqual($"{Constants.ErrorMessages.InvalidPageNumber},{Constants.ErrorMessages.InvalidPageSize}", results.Error);
        }
    }
}
