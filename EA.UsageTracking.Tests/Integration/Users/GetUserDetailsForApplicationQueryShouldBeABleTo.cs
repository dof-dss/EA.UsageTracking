using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
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
            //Arrange
            SeedData.PopulateTestData(DbContext);

            //Act
            var result = await Mediator.Send(new GetUserDetailsForApplicationQuery() { Id = Guid.Parse("b0ed668d-7ef2-4a23-a333-94ad278f4111"), });

            //Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("User 1", result.Value.Name);
        }
    }
}
