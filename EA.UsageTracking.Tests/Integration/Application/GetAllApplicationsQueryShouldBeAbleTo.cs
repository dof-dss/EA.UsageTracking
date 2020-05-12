using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Features.Applications.Queries;
using EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Queries;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Application
{
    [TestFixture]
    public class GetAllApplicationsQueryShouldBeAbleTo: BaseIntegration
    {
        public GetAllApplicationsQueryShouldBeAbleTo() : base("b0ed668d-7ef2-4a23-a333-94ad278f4111")
        { }

        [Test]
        public async Task GetOneRegisteredApp()
        {
            //Arrange
            new SeedData(DbContext).PopulateTestData();

            //Act
            var results = await Mediator.Send(new GetAllApplicationsQuery());

            //Assert
            Assert.AreEqual(1, results.Value.Data.Count());
            Assert.IsTrue(results.Value.Data.First().IsRegistered);
        }

        [Test]
        public async Task GetOneRegisteredAndOneUnregisteredApp()
        {
            //Arrange
            new SeedData(DbContext).PopulateTestData();
            var unRegisteredApp = new Core.Entities.Application
            {
                Name = "Unregistered App"
            };
            DbContext.Applications.Add(unRegisteredApp);
            DbContext.SaveChanges();

            //Act
            var results = await Mediator.Send(new GetAllApplicationsQuery());

            //Assert
            Assert.AreEqual(2, results.Value.Data.Count());
            Assert.IsTrue(results.Value.Data.Single( x => x.Name == "Application 1").IsRegistered);
            Assert.IsFalse(results.Value.Data.Single(x => x.Name == "Unregistered App").IsRegistered);
        }
    }
}
