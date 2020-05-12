using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.Infrastructure.Features.Users.Commands;
using EA.UsageTracking.Infrastructure.Features.Users.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace EA.UsageTracking.Tests.Integration.Users
{
    [TestFixture()]
    public class GetUserDetailsForApplicationQueryShouldBeABleTo: BaseIntegration
    {
        [Test]
        public async Task HandleUserNotFound()
        {
            //Act
            var result = await Mediator.Send(new GetUserDetailsForApplicationQuery { Id = Guid.NewGuid() });

            //Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoUserExists, result.Error);
        }

        [Test]
        public async Task GetUser()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            var item = new ApplicationUserDTO() {Id = Guid.NewGuid(), Name = "Test user" };

            // Act
            var getResult = await Mediator.Send(new AddApplicationUserCommand() { ApplicationUserDto = item });
            var result = await Mediator.Send(new GetUserDetailsForApplicationQuery() { Id = getResult.Value.Id, });

            //Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Test user", result.Value.Name);
        }
    }
}
