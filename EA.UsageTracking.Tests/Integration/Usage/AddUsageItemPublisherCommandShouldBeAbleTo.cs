using System;
using System.Net;
using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Usage
{
    [TestFixture]
    class AddUsageItemPublisherCommandShouldBeAbleTo: BaseIntegration
    {

        [Test]
        public async Task HandleAddUsageItemWithZeroIDEvent()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var user = new ApplicationUser() { Id = Guid.NewGuid() };
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();

            var command = new AddUsageItemPublisherCommand { ApplicationEventId = 0, ApplicationUserId = user.Id};

            // Act
            var result = await Mediator.Send(command);

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoEventExists, result.Error);
        }

        [Test]
        public async Task HandleAddUsageItemWithNoUser()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var ev = new ApplicationEvent();
            DbContext.ApplicationEvents.Add(ev);
            DbContext.SaveChanges();

            var command = new AddUsageItemPublisherCommand { ApplicationEventId = ev.Id, ApplicationUserId = Guid.Empty};

            // Act
            var result = await Mediator.Send(command);

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.EmptyGuid, result.Error);
        }

        [Test]
        public async Task AddUsageItem()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var ev = new ApplicationEvent();
            DbContext.ApplicationEvents.Add(ev);
            var user = new ApplicationUser(){ Id = Guid.NewGuid()};
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();

            var command = new AddUsageItemPublisherCommand { ApplicationEventId = ev.Id, ApplicationUserId = user.Id };

            // Act
            var result = await Mediator.Send(command);

            //Assert
            Assert.True(result.IsSuccess);
        }

    }
}
