using System;
using System.Threading;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Commands;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration
{
    [TestFixture]
    class AddUsageItemCommandShouldBeAbleTo
    {
        private readonly UsageTrackingContext _dbContext;
        private readonly IUsageTrackingContextFactory _dbContextFactory;
        private readonly Guid _tenantGuid = new Guid("b0ed668d-7ef2-4a23-a333-94ad278f45d7");

        public AddUsageItemCommandShouldBeAbleTo()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext.Request.Headers[Constants.Tenant.TenantId])
                .Returns("b0ed668d-7ef2-4a23-a333-94ad278f45d7");
            
            _dbContextFactory = new UsageTrackingContextFactory(httpContextAccessorMock.Object, Helper.CreateNewContextOptionsUsingInMemoryDatabase());
            _dbContext = _dbContextFactory.UsageTrackingContext;
        }

        [TearDown]
        public void CleanUp()
        {
            _dbContext.PurgeTable(_dbContext.UsageItems);
            _dbContext.PurgeTable(_dbContext.ApplicationUsers);
            _dbContext.PurgeTable(_dbContext.ApplicationEvents);
            _dbContext.PurgeTable(_dbContext.Applications);
            _dbContext.SaveChangesAsync().Wait();
        }

        [Test]
        public void HandleAddUsageItemWithNoApplication()
        {
            // Arrange
            var item = new UsageItemDTO { ApplicationId = 1, ApplicationEventId = 1, ApplicationUserId = 1};
            var addUsageItemCommandHandler = new AddUsageItemCommandHandler(_dbContextFactory);

            // Act
            var result = addUsageItemCommandHandler.Handle(new AddUsageItemCommand() {UsageItemDTO = item},
                CancellationToken.None);

            //Assert
            Assert.True(result.Result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoApplicationExists, result.Result.Error);
        }

        [Test]
        public void HandleAddUsageItemWithNoEvent()
        {
            // Arrange
            var app = new Application();
            _dbContext.Applications.Add(app);
            _dbContext.SaveChanges();

            var item = new UsageItemDTO { ApplicationId = app.Id, ApplicationEventId = 1, ApplicationUserId = 1 };
            var addUsageItemCommandHandler = new AddUsageItemCommandHandler(_dbContextFactory);

            // Act
            var result = addUsageItemCommandHandler.Handle(new AddUsageItemCommand() { UsageItemDTO = item },
                CancellationToken.None);

            //Assert
            Assert.True(result.Result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoEventExists, result.Result.Error);
        }

        [Test]
        public void HandleAddUsageItemWithNoUser()
        {
            // Arrange
            var app = new Application();
            _dbContext.Applications.Add(app);
            var ev = new ApplicationEvent();
            _dbContext.ApplicationEvents.Add(ev);
            _dbContext.SaveChanges();

            var item = new UsageItemDTO {ApplicationId = app.Id, ApplicationEventId = ev.Id, ApplicationUserId = 1};
            var addUsageItemCommandHandler = new AddUsageItemCommandHandler(_dbContextFactory);

            // Act
            var result = addUsageItemCommandHandler.Handle(new AddUsageItemCommand() {UsageItemDTO = item},
                CancellationToken.None);

            //Assert
            Assert.True(result.Result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoUserExists, result.Result.Error);
        }

        [Test]
        public void AddUsageItem()
        {
            // Arrange
            var app = new Application();
            _dbContext.Applications.Add(app);
            var ev = new ApplicationEvent();
            _dbContext.ApplicationEvents.Add(ev);
            var user = new ApplicationUser();
            _dbContext.ApplicationUsers.Add(user);
            _dbContext.SaveChanges();

            var item = new UsageItemDTO { ApplicationId = app.Id, ApplicationEventId = ev.Id, ApplicationUserId = user.Id };
            var addUsageItemCommandHandler = new AddUsageItemCommandHandler(_dbContextFactory);

            // Act
            var result = addUsageItemCommandHandler.Handle(new AddUsageItemCommand() { UsageItemDTO = item },
                CancellationToken.None);

            //Assert
            Assert.True(result.Result.IsSuccess);
        }

    }
}
