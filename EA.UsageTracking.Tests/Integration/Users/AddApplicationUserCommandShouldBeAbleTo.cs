using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
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
            var item = new ApplicationUserDTO(){Name = "test"};

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

            var item = new ApplicationUserDTO() { Name = "Test user" };

            // Act
            var result = await Mediator.Send(new AddApplicationUserCommand() { ApplicationUserDto = item });

            //Assert
            Assert.True(result.IsSuccess);
        }
    }
}
