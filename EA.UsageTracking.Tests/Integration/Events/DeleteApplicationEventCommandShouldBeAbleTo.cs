using System;
using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Events
{
    [TestFixture]
    class DeleteApplicationEventCommandShouldBeAbleTo: BaseIntegration
    {

        [Test]
        public async Task HandleDeleteWithNoApplication()
        {
            // Act
            var result = await Mediator.Send(new DeleteApplicationEventCommand {Id = 1});

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, result.Error);
        }

        [Test]
        public async Task HandleDeleteWithNoEvent()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            // Act
            var result = await Mediator.Send(new DeleteApplicationEventCommand { Id = 1 });

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoEventExists, result.Error);
        }


        [Test]
        public async Task DeleteApplicationEvent()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();
            var item = new ApplicationEventDTO() { Name = "Test event" };

            // Act
            var addResult = await Mediator.Send(new AddApplicationEventCommand() { ApplicationEventDto = item });
            var result = await Mediator.Send(new DeleteApplicationEventCommand { Id = addResult.Value.Id });
            var getResult = await Mediator.Send(new GetEventDetailsForApplicationQuery { Id = 1 });

            //Assert
            Assert.True(result.IsSuccess);
            Assert.IsTrue(getResult.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoEventExists, getResult.Error);
        }

        [Test]
        public async Task SoftDeleteApplicationEvent()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            var item = new ApplicationEventDTO() { Name = "Test event" };

            // Act
            var addResult = await Mediator.Send(new AddApplicationEventCommand() { ApplicationEventDto = item });
            var result = await Mediator.Send(new DeleteApplicationEventCommand { Id = addResult.Value.Id });
            var deletedEvent = await DbContext.ApplicationEvents.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == 1);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(deletedEvent);

        }

    }
}
