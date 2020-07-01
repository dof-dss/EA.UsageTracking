using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace EA.UsageTracking.Tests.Functional
{
    [TestFixture()]
    public class ApplicationUserControllerShouldBeAbleTo
    {
        private readonly HttpClient _client;

        public ApplicationUserControllerShouldBeAbleTo()
        {
            var factory = new CustomWebApplicationFactory<TestStartup>().WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot("EA.UsageTracking.Application.API");

                builder.ConfigureTestServices(services =>
                {
                    services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);
                    services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
                });
            });
            _client = factory.CreateClient();
        }

        [Test]
        public async Task GetAllUsersForApplication()
        {
            //Act
            var response = await _client.GetAsync("/api/v1/applicationUser?PageNumber=1&PageSize=100");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedResponse< ApplicationUserDTO>>(stringResponse);

            //Assert
            Assert.AreEqual(3, result.Total);
        }

        [Test]
        public async Task GetById()
        {
            //Act
            var response = await _client.GetAsync("/api/v1/applicationUser/details?id=b0ed668d-7ef2-4a23-a333-94ad278f4111");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApplicationUserDTO>(stringResponse);

            //Assert
            Assert.AreEqual("User 1", result.Name);
        }

        [Test]
        public async Task Post()
        {
            //Arrange
            var id = Guid.NewGuid();            
            var applicationUserDto = new ApplicationUserDTO() {Id = id ,Name = "Test User"};

            //Act
            var response = await _client.PostAsJsonAsync("/api/v1/applicationUser", applicationUserDto );
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApplicationUserDTO>(stringResponse);

            var getResponse = await _client.GetAsync($"/api/applicationUser/details?id={result.Id}");
            response.EnsureSuccessStatusCode();
            var getStringResponse = await response.Content.ReadAsStringAsync();
            var getResult = JsonConvert.DeserializeObject<ApplicationUserDTO>(getStringResponse);

            //Assert
            Assert.AreEqual("Test User", result.Name);
            Assert.AreEqual("Test User", getResult.Name);
            Assert.AreEqual(id, getResult.Id);
        }

        [Test]
        public async Task Put()
        {
            //Arrange
            var id = Guid.NewGuid();
            var applicationUserDto = new ApplicationUserDTO() {Id = id, Name = "User 1" };
            var postResponse = await _client.PostAsJsonAsync("/api/v1/applicationUser", applicationUserDto);
            var postStringResponse = await postResponse.Content.ReadAsStringAsync();
            var postResult = JsonConvert.DeserializeObject<ApplicationUserDTO>(postStringResponse);

            //Act
            var response = await _client.PutAsJsonAsync("/api/v1/applicationUser", 
                new ApplicationUserDTO(){Id = postResult.Id, Name = "Updated User"});
            response.EnsureSuccessStatusCode();
            var putStringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApplicationUserDTO>(putStringResponse);

            var getResponse = await _client.GetAsync($"/api/applicationUser/details?id={result.Id}");
            response.EnsureSuccessStatusCode();
            var getStringResponse = await response.Content.ReadAsStringAsync();
            var getResult = JsonConvert.DeserializeObject<ApplicationUserDTO>(getStringResponse);

            //Assert
            Assert.AreEqual("Updated User", result.Name);
            Assert.AreEqual(id, getResult.Id);
        }
        [Test]
        public async Task Delete()
        {
            //Arrange
            var id = Guid.NewGuid();
            var applicationUserDto = new ApplicationUserDTO() { Id = id, Name = "User 1" };
            var postResponse = await _client.PostAsJsonAsync("/api/v1/applicationUser", applicationUserDto);
            var postStringResponse = await postResponse.Content.ReadAsStringAsync();
            var postResult = JsonConvert.DeserializeObject<ApplicationUserDTO>(postStringResponse);

            //Act
            var response = await _client.DeleteAsync($"/api/applicationUser/{postResult.Id}");
            response.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync($"/api/applicationUser/details?id={postResult.Id}");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);

        }


    }
}
