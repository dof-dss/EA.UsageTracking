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
            DbContext.SaveChanges();

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
            var ev = new ApplicationEvent(){Name = "Test Event"};
            DbContext.ApplicationEvents.Add(ev);
            DbContext.SaveChanges();

            var item = new ApplicationEventDTO() {Id= 1, Name = "Test Event Updated" };

            // Act
            var result = await Mediator.Send(new UpdateApplicationEventCommand() { ApplicationEventDto = item });
            var getResult = await Mediator.Send(new GetEventDetailsForApplicationQuery { Id = 1 });

            //Assert
            Assert.True(result.IsSuccess);
            Assert.True(getResult.IsSuccess);
            Assert.AreEqual(item.Name, getResult.Value.Name);
        }

    }
}
