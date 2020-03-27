using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using WebMotions.Fake.Authentication.JwtBearer;

namespace EA.UsageTracking.Tests.Functional
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly ServiceProvider _serviceProvider;

        public CustomWebApplicationFactory()
        {
            _serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(null)
                .UseStartup<TStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                MockHttpContext(services);
                AddInMemoryDbOptions(services);
                SeedTestData(services.BuildServiceProvider());
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer(); ;
            });
        }

        private void MockHttpContext(IServiceCollection services)
        {
            var mockAccessor = new Mock<IHttpContextAccessor>();
            mockAccessor.Setup(x => x.HttpContext.User.Claims).Returns(new List<Claim>() { new Claim("client_id", Guid.NewGuid().ToString()) });
            mockAccessor.Setup(x => x.HttpContext.Request.Scheme).Returns("http");
            mockAccessor.Setup(x => x.HttpContext.Request.Host).Returns(new HostString("test"));
            services.AddSingleton<IHttpContextAccessor>(x => mockAccessor.Object);
        }

        private void AddInMemoryDbOptions(IServiceCollection services)
        {
            services.AddSingleton(x =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<UsageTrackingContext>();
                optionsBuilder.UseInMemoryDatabase("InMemoryDbForTesting");
                optionsBuilder.UseInternalServiceProvider(_serviceProvider);

                return optionsBuilder.Options;
            });
        }

        private void SeedTestData(ServiceProvider sp)
        {
            // Create a scope to obtain a reference to the database
            // context (UsageTrackingContext).
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                var options = scopedServices.GetRequiredService<DbContextOptions<UsageTrackingContext>>();
                var httpContextAccessorMock = scopedServices.GetRequiredService<IHttpContextAccessor>();

                var db = new UsageTrackingContextFactory(httpContextAccessorMock, options);

                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                db.UsageTrackingContext.Database.EnsureCreated();

                try
                {
                    // Seed the database with test data.
                    new SeedData(db.UsageTrackingContext).PopulateTestData();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the " +
                                        $"database with test messages. Error: {ex.Message}");
                }
            }
        }
    }
}