using System;
using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.Infrastructure.Features.Users.Commands;
using EA.UsageTracking.Infrastructure.Features.Users.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Users
{
    [TestFixture()]
    public class UpdateApplicationUserCommandShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task HandleUpdateApplicationUserWithNoApplication()
        {
            // Arrange
            var item = new ApplicationUserDTO() { Name = "Test User" };

            // Act
            var result = await Mediator.Send(new UpdateApplicationUserCommand() { ApplicationUserDTO = item });

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, result.Error);
        }

        [Test]
        public async Task HandleUpdateApplicationUserWithNoUser()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            // Act
            var result = await Mediator.Send(new UpdateApplicationUserCommand() { ApplicationUserDTO = new ApplicationUserDTO(){Id = Guid.NewGuid()} });

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoUserExists, result.Error);
        }


        [Test]
        public async Task UpdateApplicationEvent()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var user = new ApplicationUser() { Name = "Test User" };
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();

            var item = new ApplicationUserDTO() { Id = user.Id, Name = "Test User Updated" };

            // Act
            var result = await Mediator.Send(new UpdateApplicationUserCommand() { ApplicationUserDTO = item });
            var getResult = await Mediator.Send(new GetUserDetailsForApplicationQuery { Id = item.Id });

            //Assert
            Assert.True(result.IsSuccess);
            Assert.True(getResult.IsSuccess);
            Assert.AreEqual(item.Name, getResult.Value.Name);
        }
    }
}
