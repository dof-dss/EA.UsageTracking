using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using WebMotions.Fake.Authentication.JwtBearer;

namespace EA.UsageTracking.Tests.Functional
{
    [TestFixture]
    public class UsageItemsControllerShouldBeAbleTo
    {
        private readonly HttpClient _client;

        public UsageItemsControllerShouldBeAbleTo()
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
        public async Task GetAllForApplication()
        {
            //Act
            var response = await _client.GetAsync("/api/applicationUsage?PageNumber=1&PageSize=2");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedResponse<UsageItemDTO>>(stringResponse);

            //Assert
            Assert.AreEqual(2, result.Data.Count());
        }

        [Test]
        public async Task GetById()
        {
            //Act
            var response = await _client.GetAsync("/api/applicationUsage/details?id=1");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UsageItemDTO>(stringResponse);

            //Assert
            Assert.AreEqual("Application 1", result.ApplicationName);
            Assert.AreEqual("Event 1", result.ApplicationEventName);
            Assert.AreEqual("User 1", result.ApplicationUserName);
        }

        [Test]
        public async Task GetByUser()
        {
            //Act
            var response = await _client.GetAsync("/api/applicationUsage/user?id=b0ed668d-7ef2-4a23-a333-94ad278f4111");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedResponse<UsageItemDTO>>(stringResponse);

            //Assert
            Assert.AreEqual("User 1", result.Data.First().ApplicationUserName);
        }

        [Test]
        public async Task PostUsageItem()
        {
            //Arrange
            var usageItemDTO = new UsageItemDTO{ApplicationId = 1, 
                ApplicationEventId = 1, ApplicationUserId = Guid.Parse("b0ed668d-7ef2-4a23-a333-94ad278f4111")};

            //Act
            var response = await _client.PostAsJsonAsync("/api/applicationUsage", usageItemDTO);
            response.EnsureSuccessStatusCode();

            //Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
