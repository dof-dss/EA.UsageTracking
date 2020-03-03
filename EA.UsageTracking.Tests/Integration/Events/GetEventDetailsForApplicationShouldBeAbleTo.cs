using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Events
{
    [TestFixture()]
    public class GetEventDetailsForApplicationShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task HandleEventNotFound()
        {
            //Act
            var result = await Mediator.Send(new GetEventDetailsForApplicationQuery {Id = 1});

            //Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoEventExists, result.Error);
        }

        [Test]
        public async Task GetEvent()
        {
            //Arrange
            SeedData.PopulateTestData(DbContext);

            //Act
            var result = await Mediator.Send(new GetEventDetailsForApplicationQuery { Id = 1 });

            //Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Event 1", result.Value.Name);
        }
    }
}
