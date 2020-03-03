﻿using System;
using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Usage
{
    [TestFixture]
    class AddUsageItemCommandShouldBeAbleTo: BaseIntegration
    {

        [Test]
        public async Task HandleAddUsageItemWithNoApplication()
        {
            // Arrange
            var item = new UsageItemDTO { ApplicationId = 1, ApplicationEventId = 1, ApplicationUserId = Guid.NewGuid()};

            // Act
            var result = await Mediator.Send(new AddUsageItemCommand {UsageItemDTO = item});

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
            DbContext.SaveChanges();

            var item = new UsageItemDTO { ApplicationId = app.Id, ApplicationEventId = 1, ApplicationUserId = Guid.NewGuid() };

            // Act
            var result = await Mediator.Send(new AddUsageItemCommand() { UsageItemDTO = item });

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

            var item = new UsageItemDTO {ApplicationId = app.Id, ApplicationEventId = ev.Id, ApplicationUserId = Guid.NewGuid()};

            // Act
            var result = await Mediator.Send(new AddUsageItemCommand() {UsageItemDTO = item});

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

            var item = new UsageItemDTO { ApplicationId = app.Id, ApplicationEventId = ev.Id, ApplicationUserId = user.Id };

            // Act
            var result = await Mediator.Send(new AddUsageItemCommand() { UsageItemDTO = item });

            //Assert
            Assert.True(result.IsSuccess);
        }

    }
}
