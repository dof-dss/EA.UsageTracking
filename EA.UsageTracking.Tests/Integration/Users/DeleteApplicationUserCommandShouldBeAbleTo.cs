using System;
using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.Infrastructure.Features.Users.Commands;
using EA.UsageTracking.Infrastructure.Features.Users.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Users
{
    [TestFixture()]
    public class DeleteApplicationUserCommandShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task HandleDeleteApplicationUserWithNoApplication()
        {
            // Act
            var result = await Mediator.Send(new DeleteApplicationUserCommand() { Id = Guid.Parse("b0ed668d-7ef2-4a23-a333-94ad278f4111") });

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, result.Error);
        }

        [Test]
        public async Task HandleDeleteUserWithNoUser()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            // Act
            var result = await Mediator.Send(new DeleteApplicationUserCommand() { Id = Guid.Parse("b0ed668d-7ef2-4a23-a333-94ad278f4111") });

            //Assert
            Assert.True(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoUserExists, result.Error);
        }


        [Test]
        public async Task DeleteApplicationUser()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var user = new ApplicationUser(){Id = Guid.NewGuid()};
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();

            // Act
            var result = await Mediator.Send(new DeleteApplicationUserCommand() { Id = user.Id });
            var getResult = await Mediator.Send(new GetUserDetailsForApplicationQuery { Id = user.Id });

            //Assert
            Assert.True(result.IsSuccess);
            Assert.IsTrue(getResult.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoUserExists, getResult.Error);
        }

        [Test]
        public async Task SoftDeleteApplicationUser()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var user = new ApplicationUser() { Id = Guid.NewGuid() };
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();

            // Act
            var result = await Mediator.Send(new DeleteApplicationUserCommand { Id = user.Id});
            var deletedEvent = await DbContext.ApplicationUsers.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == user.Id);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(deletedEvent);

        }
    }
}
