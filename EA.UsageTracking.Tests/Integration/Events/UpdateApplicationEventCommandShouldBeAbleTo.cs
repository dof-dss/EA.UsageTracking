using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Events
{
    [TestFixture]
    class UpdateApplicationEventCommandShouldBeAbleTo: BaseIntegration
    {

        [Test]
        public async Task HandleUpdateApplicationEventWithNoApplication()
        {
            // Arrange
            var item = new ApplicationEventDTO() { Name = "Test" };

            // Act
            var result = await Mediator.Send(new UpdateApplicationEventCommand() { ApplicationEventDto = item });

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, result.Error);
        }

        [Test]
        public async Task HandleUpdateApplicationEventWithNoEvent()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await Mediator.Send(new UpdateApplicationEventCommand() { ApplicationEventDto = new ApplicationEventDTO(){Id = 1} });

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoEventExists, result.Error);
        }


        [Test]
        public async Task UpdateApplicationEvent()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            await DbContext.SaveChangesAsync();
            var originalEvent = new ApplicationEventDTO() { Name = "Test event" };
            var addResult = await Mediator.Send(new AddApplicationEventCommand() { ApplicationEventDto = originalEvent });
            var updateEvent = new ApplicationEventDTO() {Id= addResult.Value.Id, Name = "Test Event Updated" };

            // Act
            var result = await Mediator.Send(new UpdateApplicationEventCommand() { ApplicationEventDto = updateEvent });
            var getResult = await Mediator.Send(new GetEventDetailsForApplicationQuery { Id = result.Value.Id });

            //Assert
            Assert.True(result.IsSuccess);
            Assert.True(getResult.IsSuccess);
            Assert.AreEqual(updateEvent.Name, getResult.Value.Name);
        }

    }
}
