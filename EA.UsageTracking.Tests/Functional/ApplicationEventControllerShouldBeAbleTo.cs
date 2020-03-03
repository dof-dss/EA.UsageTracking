using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.SharedKernel.Constants;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace EA.UsageTracking.Tests.Functional
{
    [TestFixture()]
    public class ApplicationEventControllerShouldBeAbleTo
    {
        private readonly HttpClient _client;

        public ApplicationEventControllerShouldBeAbleTo()
        {
            _client = new CustomWebApplicationFactory<Startup>().CreateClient();
            _client.DefaultRequestHeaders.Add(Constants.Tenant.TenantId, "b0ed668d-7ef2-4a23-a333-94ad278f45d7");
        }

        [Test]
        public async Task GetAllEventsForApplication()
        {
            //Act
            var response = await _client.GetAsync("/api/applicationEvent?PageNumber=1&PageSize=100");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ApplicationEventDTO>>(stringResponse).ToList();

            //Assert
            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public async Task GetById()
        {
            //Act
            var response = await _client.GetAsync("/api/applicationEvent/details?id=1");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApplicationEventDTO>(stringResponse);

            //Assert
            Assert.AreEqual("Event 1", result.Name);
        }

        [Test]
        public async Task Post()
        {
            //Arrange
            var applicationEventDTO = new ApplicationEventDTO() {Name = "Event 1"};

            //Act
            var response = await _client.PostAsJsonAsync("/api/applicationEvent", applicationEventDTO );
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApplicationEventDTO>(stringResponse);

            var getResponse = await _client.GetAsync($"/api/applicationEvent/details?id={result.Id}");
            response.EnsureSuccessStatusCode();
            var getStringResponse = await response.Content.ReadAsStringAsync();
            var getResult = JsonConvert.DeserializeObject<ApplicationEventDTO>(getStringResponse);

            //Assert
            Assert.AreEqual("Event 1", result.Name);
            Assert.AreEqual("Event 1", getResult.Name);
        }

        [Test]
        public async Task Put()
        {
            //Arrange
            var applicationEventDTO = new ApplicationEventDTO() { Name = "Event 1" };
            var postResponse = await _client.PostAsJsonAsync("/api/applicationEvent", applicationEventDTO);
            var postStringResponse = await postResponse.Content.ReadAsStringAsync();
            var postResult = JsonConvert.DeserializeObject<ApplicationEventDTO>(postStringResponse);

            //Act
            var response = await _client.PutAsJsonAsync("/api/applicationEvent", 
                new ApplicationEventDTO(){Id = postResult.Id, Name = "Updated Event"});
            response.EnsureSuccessStatusCode();
            var putStringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApplicationEventDTO>(putStringResponse);

            var getResponse = await _client.GetAsync($"/api/applicationEvent/details?id={result.Id}");
            response.EnsureSuccessStatusCode();
            var getStringResponse = await response.Content.ReadAsStringAsync();
            var getResult = JsonConvert.DeserializeObject<ApplicationEventDTO>(getStringResponse);

            //Assert
            Assert.AreEqual("Updated Event", result.Name);
        }
        [Test]
        public async Task Delete()
        {
            //Arrange
            var applicationEventDTO = new ApplicationEventDTO() { Name = "Event 1" };
            var postResponse = await _client.PostAsJsonAsync("/api/applicationEvent", applicationEventDTO);
            var postStringResponse = await postResponse.Content.ReadAsStringAsync();
            var postResult = JsonConvert.DeserializeObject<ApplicationEventDTO>(postStringResponse);

            //Act
            var response = await _client.DeleteAsync($"/api/applicationEvent/{postResult.Id}");
            response.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync($"/api/applicationEvent/details?id={postResult.Id}");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);

        }


    }
}
