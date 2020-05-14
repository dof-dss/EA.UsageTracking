using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
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
        protected UsageTrackingContext DbContext;
        protected  IMediator Mediator;
        private  Mock<IHttpContextAccessor> _mockAccessor;
        private ServiceCollection _services = new ServiceCollection();
        private string _tenantId;

        protected IUsageTrackingContextFactory UsageTrackingContextFactory { get; set; }

        public  BaseIntegration(): this(Guid.NewGuid())
        { }

        public BaseIntegration(string userId): this()
        {
            _mockAccessor.Setup(x => x.HttpContext.User.Claims).Returns(new List<Claim>()
            {
                new Claim("client_id", _tenantId),
                new Claim("username", userId)
            });
        }

        public BaseIntegration(Guid tenantGuid)
        {
            _tenantId = tenantGuid.ToString();
            _mockAccessor = new Mock<IHttpContextAccessor>();
            _mockAccessor.Setup(x => x.HttpContext.User.Claims).Returns(new List<Claim>()
            {
                new Claim("client_id", tenantGuid.ToString())
            });
            _mockAccessor.Setup(x => x.HttpContext.Request.Scheme).Returns("http");
            _mockAccessor.Setup(x => x.HttpContext.Request.Host).Returns(new HostString("test"));
            _services.AddSingleton<IHttpContextAccessor>(x => _mockAccessor.Object);

            var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
            mockConnectionMultiplexer.Setup(x => x.GetSubscriber(null)).Returns(new FakeSubscriber());
            _services.AddSingleton(mockConnectionMultiplexer.Object);

            _services.AddLogging();

            _services.AddAutoMapper(typeof(UsageTrackingContext).GetTypeInfo().Assembly);
            _services.AddMediatR(typeof(UsageTrackingContext).GetTypeInfo().Assembly,
                typeof(Result).GetTypeInfo().Assembly,
                typeof(UsageItem).GetTypeInfo().Assembly);

            _services.AddSingleton<IUriService, UriService>();
            _services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            _services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
            _services.AddTransient<IUsageTrackingContextFactory, UsageTrackingContextFactory>();
        }

        [SetUp]
        public void Setup()
        {
            var randomInMemoryDbName = _tenantId + "-" + Guid.NewGuid();
            _services.AddDbContext<UsageTrackingContext>(options => options.UseInMemoryDatabase(randomInMemoryDbName));
            _services.AddSingleton(sp =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<UsageTrackingContext>();
                return optionsBuilder.UseInMemoryDatabase(randomInMemoryDbName).Options;
            });

            var servicesProvider = _services.BuildServiceProvider();

            Mediator = servicesProvider.GetRequiredService<IMediator>();
            UsageTrackingContextFactory = servicesProvider.GetRequiredService<IUsageTrackingContextFactory>();
            DbContext = UsageTrackingContextFactory.UsageTrackingContext;
        }

        //[TearDown]
        //public void CleanUp()
        //{
        //    DbContext.PurgeTable(DbContext.UsageItems);
        //    DbContext.PurgeTable(DbContext.ApplicationUsers);
        //    DbContext.PurgeTable(DbContext.ApplicationEvents);
        //    DbContext.PurgeTable(DbContext.Applications);
        //}
    }
}
