using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Events
{
    [TestFixture]
    class AddApplicationEventCommandShouldBeAbleTo: BaseIntegration
    {

        [Test]
        public async Task HandleAddUsageItemWithNoApplication()
        {
            // Arrange
            var item = new ApplicationEventDTO(){Name = "Test"};

            // Act
            var result = await Mediator.Send(new AddApplicationEventCommand(){ApplicationEventDto = item});

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, result.Error);
        }


        [Test]
        public async Task AddApplicationEvent()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            var item = new ApplicationEventDTO() { Name = "Test event"};

            // Act
            var result = await Mediator.Send(new AddApplicationEventCommand() { ApplicationEventDto = item });

            //Assert
            Assert.True(result.IsSuccess);
        }

    }
}
