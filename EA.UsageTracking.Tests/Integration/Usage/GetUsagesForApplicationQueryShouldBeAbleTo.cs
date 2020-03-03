using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Features.Usages.Queries;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Usage
{
    [TestFixture()]
    public class GetUsagesForApplicationQueryShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task GetNoUsages()
        {
            //Act
            var results = await Mediator.Send(new GetUsagesForApplicationQuery { });

            //Assert
            Assert.AreEqual(0, results.Value.Count);
        }

        [Test]
        public async Task GetUsages()
        {
            //Arrange
            SeedData.PopulateTestData(DbContext);

            //Act
            var results = await Mediator.Send(new GetUsagesForApplicationQuery { });

            //Assert
            Assert.AreEqual(2, results.Value.Count);
        }

    }
}
