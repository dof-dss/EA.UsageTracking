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
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
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
            var addUsageItemPublisherCommand = new AddUsageItemPublisherCommand
            {
                ApplicationEventId = 1,
                IdentityToken = "eyJraWQiOiJoUk1YMXZUQU8zUmpXeHloVG4zZUpSblFZejNpMWZTT044WjV3d0VCVGJrPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoiVUotcEl0VEZmTElpazJLdFF4VVdtQSIsInN1YiI6IjUzZWZiM2M0LWY4MzctNGUxMS1hNjIyLTYwYTQzYjBlMTJhNCIsImNvZ25pdG86Z3JvdXBzIjpbIkFkbWluIl0sImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0yLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMl9RdGdvZ0g5MXYiLCJjb2duaXRvOnVzZXJuYW1lIjoiNTNlZmIzYzQtZjgzNy00ZTExLWE2MjItNjBhNDNiMGUxMmE0IiwiY29nbml0bzpyb2xlcyI6WyJhcm46YXdzOmlhbTo6MTExMDk0ODE4MjE3OnJvbGVcL0FQSUdhdGV3YXlJbnZva2VSb2xlIl0sImF1ZCI6IjNqMGZla281YWkwYjU4ZDlrOGVjZGp1cGdhIiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1ODY3ODk1NjcsImV4cCI6MTU4Njc5MzE2NywiaWF0IjoxNTg2Nzg5NTY3LCJlbWFpbCI6ImdyYWhhbS5tY3ZlYUBnbWFpbC5jb20ifQ.BSXjwtmJ4yJf0wcIIr1_JYMng3Yhw6IHrHy0507xO21zzDhWf9wsq2kkvhn0nSMQDTw5sYj6pMAJFNkGMKchk8poT5vgspwhCT-Io1xKVN8a9xVdoxxXJkZrHxv8lw-JoxnC_giLOrvq0XRDE6JsaoLD6EQnxXQKaJ5IzG0784wqMUNOLA7QiHAhFOd5IbvE6zMzbc5jsPIMFRXKF_t71y08CtjYw40wV4Lk8FAvANBwYkPobKzYEZEGNwYQFn3rl5dx4ohJfiennUzVfntDuwh4FELqPSR9dicStUFwBaHvWMdMYSKHRv8GeTpYPY9c2J3JuXrB3z8sicqdt_UhjA"
            };

            //Act
            var response = await _client.PostAsJsonAsync("/api/applicationUsage", addUsageItemPublisherCommand);
            response.EnsureSuccessStatusCode();

            //Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
