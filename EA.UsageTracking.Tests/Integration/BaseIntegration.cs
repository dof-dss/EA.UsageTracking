using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration
{
    [TestFixture]
    public class BaseIntegration
    {
        protected readonly UsageTrackingContext DbContext;
        protected readonly IMediator Mediator;
        protected IContainer Container { get; set; }

        protected IUsageTrackingContextFactory UsageTrackingContextFactory { get; }

        public  BaseIntegration(): this(Guid.NewGuid())
        { }

        public BaseIntegration(Guid tenantGuid)
        {
            var builder = new ContainerBuilder();

            var mockAccessor = new Mock<IHttpContextAccessor>();
            mockAccessor.Setup(x => x.HttpContext.Request.Headers[Constants.Tenant.TenantId]).Returns(tenantGuid.ToString());
            mockAccessor.Setup(x => x.HttpContext.Request.Scheme).Returns("http");
            mockAccessor.Setup(x => x.HttpContext.Request.Host).Returns(new HostString("test"));
            builder.RegisterInstance(mockAccessor.Object).As<IHttpContextAccessor>();

            var optionsBuilder = new DbContextOptionsBuilder<UsageTrackingContext>();
            optionsBuilder.UseInMemoryDatabase("InMemoryDbForTesting");
            builder.RegisterInstance(optionsBuilder.Options).As<DbContextOptions<UsageTrackingContext>>();

            builder.RegisterType<UsageTrackingContextFactory>().As<IUsageTrackingContextFactory>();
            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            var coreAssembly = Assembly.GetAssembly(typeof(UsageItem));
            var infrastructureAssembly = Assembly.GetAssembly(typeof(UsageTrackingContext));
            var sharedKernelAssembly = Assembly.GetAssembly(typeof(Constants));

            var autoMapperProfiles = infrastructureAssembly.DefinedTypes
                .Where(x => typeof(Profile).IsAssignableFrom(x) && x.IsPublic && !x.IsAbstract)
                .Select(p => (Profile)Activator.CreateInstance(p)).ToList();

            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                foreach (var profile in autoMapperProfiles)
                {
                    cfg.AddProfile(profile);
                }
            }));

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>().InstancePerLifetimeScope();

            builder.RegisterType<UriService>().As<IUriService>().SingleInstance();

            builder.RegisterAssemblyTypes(sharedKernelAssembly, coreAssembly, infrastructureAssembly).AsImplementedInterfaces();

            Container = builder.Build();
            Mediator = Container.Resolve<IMediator>();

            UsageTrackingContextFactory = Container.Resolve<IUsageTrackingContextFactory>();
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
