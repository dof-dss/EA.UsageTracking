using System;
using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Usage
{
    [TestFixture]
    class AddUsageItemSubscriberCommandShouldBeAbleTo: BaseIntegration
    {

        [Test]
        public async Task HandleAddUsageItemWithNoApplication()
        {
            // Arrange
            var command = new AddUsageItemSubscriberCommand { ApplicationEventId = 1, ApplicationUserId = Guid.NewGuid()};

            // Act
            var result = await Mediator.Send(command);

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, result.Error);
        }

        [Test]
        public async Task HandleAddUsageItemWithNoEvent()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var user = new ApplicationUser() { Id = Guid.NewGuid() };
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();

            var command = new AddUsageItemSubscriberCommand { ApplicationEventId = 1, ApplicationUserId = user.Id, TenantId = DbContext.TenantId };

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

            var command = new AddUsageItemSubscriberCommand { ApplicationEventId = ev.Id, ApplicationUserId = Guid.NewGuid(), TenantId = DbContext.TenantId };

            // Act
            var result = await Mediator.Send(command);

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoUserExists, result.Error);
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

            var command = new AddUsageItemSubscriberCommand { ApplicationEventId = ev.Id, ApplicationUserId = user.Id, TenantId = DbContext.TenantId};

            // Act
            var result = await Mediator.Send(command);

            //Assert
            Assert.True(result.IsSuccess);
        }

    }
}
