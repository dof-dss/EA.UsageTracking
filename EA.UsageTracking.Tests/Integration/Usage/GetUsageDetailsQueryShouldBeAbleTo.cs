using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
using EA.UsageTracking.Infrastructure.Features.Usages.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Usage
{
    [TestFixture]
    public class GetUsageDetailsQueryShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task GetUsageDetails()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var ev = new ApplicationEvent();
            DbContext.ApplicationEvents.Add(ev);
            var user = new ApplicationUser();
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();

            var item = new UsageItemDTO { ApplicationId = app.Id, ApplicationEventId = ev.Id, ApplicationUserId = user.Id };

            //Act
            var addResult = await Mediator.Send(new AddUsageItemCommand() { UsageItemDTO = item });
            var result = await Mediator.Send(new GetUsageDetailsQuery {Id = addResult.Value.Id});

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Test]
        public async Task HandleNoUsageDetails()
        {
            //Act
            var result = await Mediator.Send(new GetUsageDetailsQuery { Id = 1 });

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoItemExists, result.Error);
        }

    }
}
