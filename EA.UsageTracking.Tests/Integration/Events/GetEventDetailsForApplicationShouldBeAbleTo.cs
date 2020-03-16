using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Events
{
    [TestFixture()]
    public class GetEventDetailsForApplicationShouldBeAbleTo: BaseIntegration
    {
        [Test]
        public async Task HandleNoTenant()
        {
            //Act
            var result = await Mediator.Send(new GetEventDetailsForApplicationQuery { Id = 66666667 });

            //Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoTenantExists, result.Error);
        }

        [Test]
        public async Task HandleEventNotFound()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            //Act
            var result = await Mediator.Send(new GetEventDetailsForApplicationQuery {Id = 66666667});

            //Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(Constants.ErrorMessages.NoEventExists, result.Error);
        }

        [Test]
        public async Task GetEvent()
        {
            //Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();
            var item = new ApplicationEventDTO() { Name = "Test event" };

            // Act
            var addResult = await Mediator.Send(new AddApplicationEventCommand() { ApplicationEventDto = item });
            var result = await Mediator.Send(new GetEventDetailsForApplicationQuery { Id = addResult.Value.Id });

            //Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Test event", result.Value.Name);
        }
    }
}
