using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.API;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace EA.UsageTracking.Tests.Functional
{
    [TestFixture]
    public class UsageItemsControllerShouldBeAbleTo
    {
        private readonly HttpClient _client;

        public UsageItemsControllerShouldBeAbleTo()
        {
            _client = new CustomWebApplicationFactory<Startup>().CreateClient();
        }

        [Test]
        public async Task GetAllForApplication()
        {
            //Act
            var response = await _client.GetAsync("/api/usageItems/application/1/1/100");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<UsageItemDTO>>(stringResponse).ToList();

            //Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetById()
        {
            //Act
            var response = await _client.GetAsync("/api/usageItems/1");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UsageItemDTO>(stringResponse);

            //Assert
            Assert.AreEqual("Application 1", result.ApplicationName);
            Assert.AreEqual("Event 1", result.ApplicationEventName);
            Assert.AreEqual("User 1", result.ApplicationUserName);
        }

        [Test]
        public async Task PostUsageItem()
        {
            //Arrange
            var usageItemDTO = new UsageItemDTO{ApplicationId = 1, ApplicationEventId = 1, ApplicationUserId = 3};

            //Act
            var response = await _client.PostAsJsonAsync("/api/UsageItems", usageItemDTO);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UsageItemDTO>(stringResponse);

            //Assert
            Assert.AreEqual("Application 1", result.ApplicationName);
            Assert.AreEqual("Event 1", result.ApplicationEventName);
            Assert.AreEqual("User 3", result.ApplicationUserName);
        }
    }
}
