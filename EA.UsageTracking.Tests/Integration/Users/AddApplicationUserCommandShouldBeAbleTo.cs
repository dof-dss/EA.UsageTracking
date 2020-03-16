using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Users.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Users
{
    [TestFixture()]
    public class AddApplicationUserCommandShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task HandleAddUserWithNoApplication()
        {
            // Arrange
            var item = new ApplicationUserDTO(){Id = Guid.NewGuid(), Name = "test"};

            // Act
            var result = await Mediator.Send(new AddApplicationUserCommand() { ApplicationUserDto = item });

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, result.Error);
        }


        [Test]
        public async Task AddApplicationUser()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            var Id = Guid.NewGuid();
            var item = new ApplicationUserDTO() {Id = Id, Name = "Test user" };

            // Act
            var result = await Mediator.Send(new AddApplicationUserCommand() { ApplicationUserDto = item });

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Test]
        public async Task AddApplicationUserToMultipleApps()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            DbContext.TenantId = Guid.NewGuid();
            var app2 = new Core.Entities.Application
            {
                UserToApplications = new List<UserToApplication>
                {
                    new UserToApplication()
                    {
                        User = new ApplicationUser {Id = Guid.NewGuid(), Name = "Test User"}
                    }
                }
            };

            DbContext.Applications.Add(app2);
            DbContext.SaveChanges();

            var item = new ApplicationUserDTO() { Id = app2.UserToApplications.First().UserId, Name = "Test user" };

            // Act
            var result = await Mediator.Send(new AddApplicationUserCommand() { ApplicationUserDto = item });

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Test]
        public async Task HandleAddApplicationUserWithNoGuid()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            var item = new ApplicationUserDTO() { Name = "Test user" };

            // Act
            var result = await Mediator.Send(new AddApplicationUserCommand() { ApplicationUserDto = item });

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.EmptyGuid, result.Error);
        }
    }
}
