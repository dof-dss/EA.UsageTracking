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

            var command = new AddUsageItemSubscriberCommand { ApplicationEventId = ev.Id, ApplicationUserId = user.Id, TenantId = DbContext.TenantId};

            //Act
            var addResult = await Mediator.Send(command);
            var result = await Mediator.Send(new GetUsageDetailsQuery {Id = addResult.Value});

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
