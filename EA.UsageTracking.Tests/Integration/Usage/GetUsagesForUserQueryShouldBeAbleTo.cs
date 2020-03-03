using System;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Features.Usages.Queries;
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
            SeedData.PopulateTestData(DbContext);

            //Act
            var results = await Mediator.Send(new GetUsagesForUserQuery {Id = Guid.Parse("b0ed668d-7ef2-4a23-a333-94ad278f4111") });

            //Assert
            Assert.AreEqual(1, results.Value.Count);
        }

        [Test]
        public async Task GetNoUsagesForUser()
        {
            //Act
            var results = await Mediator.Send(new GetUsagesForUserQuery { Id = Guid.NewGuid() });

            //Assert
            Assert.AreEqual(0, results.Value.Count);
        }
    }
}
