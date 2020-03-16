using System;
using System.Linq;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace EA.UsageTracking.Tests.Functional
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        private readonly ServiceProvider _serviceProvider;

        public CustomWebApplicationFactory()
        {
            _serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                AddInMemoryDbOptions(services);

                var sp = services.BuildServiceProvider();

                SeedTestData(sp);
            });
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
                var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
                httpContextAccessorMock.Setup(x => x.HttpContext.Request.Headers[Constants.Tenant.TenantId])
                    .Returns("b0ed668d-7ef2-4a23-a333-94ad278f45d7");

                var db = new UsageTrackingContextFactory(httpContextAccessorMock.Object, options);

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