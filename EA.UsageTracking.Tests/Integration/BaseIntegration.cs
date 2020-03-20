using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Behaviors;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace EA.UsageTracking.Tests.Integration
{
    [TestFixture]
    public class BaseIntegration
    {
        protected readonly UsageTrackingContext DbContext;
        protected readonly IMediator Mediator;

        protected IUsageTrackingContextFactory UsageTrackingContextFactory { get; }

        public  BaseIntegration(): this(Guid.NewGuid())
        { }

        public BaseIntegration(Guid tenantGuid)
        {
            var services = new ServiceCollection();

            var mockAccessor = new Mock<IHttpContextAccessor>();
            mockAccessor.Setup(x => x.HttpContext.Request.Headers[Constants.Tenant.TenantId]).Returns(tenantGuid.ToString());
            mockAccessor.Setup(x => x.HttpContext.Request.Scheme).Returns("http");
            mockAccessor.Setup(x => x.HttpContext.Request.Host).Returns(new HostString("test"));
            services.AddSingleton<IHttpContextAccessor>(x => mockAccessor.Object);

            var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
            mockConnectionMultiplexer.Setup(x => x.GetSubscriber(null)).Returns(new FakeSubscriber());
            services.AddSingleton(mockConnectionMultiplexer.Object);

            services.AddLogging();

            services.AddDbContext<UsageTrackingContext>(options => options.UseInMemoryDatabase("InMemoryDbForTesting"));
            services.AddSingleton(sp =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<UsageTrackingContext>();
                return optionsBuilder.UseInMemoryDatabase("InMemoryDbForTesting").Options;
            });

            services.AddAutoMapper(typeof(UsageTrackingContext).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(UsageTrackingContext).GetTypeInfo().Assembly,
                typeof(Result).GetTypeInfo().Assembly,
                typeof(UsageItem).GetTypeInfo().Assembly);

            services.AddSingleton<IUriService, UriService>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
            services.AddTransient<IUsageTrackingContextFactory, UsageTrackingContextFactory>();

            var servicesProvider = services.BuildServiceProvider();

            Mediator = servicesProvider.GetRequiredService<IMediator>();

            UsageTrackingContextFactory = servicesProvider.GetRequiredService<IUsageTrackingContextFactory>();
            DbContext = UsageTrackingContextFactory.UsageTrackingContext;
        }

        [TearDown]
        public void CleanUp()
        {
            DbContext.PurgeTable(DbContext.UsageItems);
            DbContext.PurgeTable(DbContext.ApplicationUsers);
            DbContext.PurgeTable(DbContext.ApplicationEvents);
            DbContext.PurgeTable(DbContext.Applications);
        }
    }
}
