using System;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.UsagePerUser
{
    [TestFixture]
    public class GetUsagesShouldBeAbleTo: BaseIntegration
    {
        public GetUsagesShouldBeAbleTo(): base("b0ed668d-7ef2-4a23-a333-94ad278f4111")
        { }

        [Test]
        public async Task GetUsagesForUser()
        {
            //Arrange
            new SeedData(DbContext).PopulateTestData();

            //Act
            var results = await Mediator.Send(new GetUsagesQuery());

            //Assert
            Assert.AreEqual(1, results.Value.Data.Count());
        }

        [Test]
        public async Task HandleGetUsagesWithInvalidPageNumber()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetUsagesQuery { PageNumber = 0 });

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
            var results = await Mediator.Send(new GetUsagesQuery { PageSize = 0 });

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
            var results = await Mediator.Send(new GetUsagesQuery { PageNumber = 0, PageSize = 0 });

            //Assert
            Assert.AreEqual($"{Constants.ErrorMessages.InvalidPageNumber},{Constants.ErrorMessages.InvalidPageSize}", results.Error);
        }
    }
}
