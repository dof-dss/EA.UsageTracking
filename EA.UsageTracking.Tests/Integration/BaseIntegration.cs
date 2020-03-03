using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using AutoMapper;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
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

        public BaseIntegration()
        {
            var builder = new ContainerBuilder();

            var mockAccessor = new Mock<IHttpContextAccessor>();
            mockAccessor.Setup(x => x.HttpContext.Request.Headers[Constants.Tenant.TenantId]).Returns("b0ed668d-7ef2-4a23-a333-94ad278f45d7");
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

            builder.RegisterAssemblyTypes(sharedKernelAssembly, coreAssembly, infrastructureAssembly).AsImplementedInterfaces();

            var container = builder.Build();
            Mediator = container.Resolve<IMediator>();

            var usageTrackingContextFactory = container.Resolve<IUsageTrackingContextFactory>();
            DbContext = usageTrackingContextFactory.UsageTrackingContext;
        }

        [TearDown]
        public void CleanUp()
        {
            DbContext.PurgeTable<UsageItem,int>(DbContext.UsageItems);
            DbContext.PurgeTable<ApplicationUser, Guid>(DbContext.ApplicationUsers);
            DbContext.PurgeTable<ApplicationEvent,int>(DbContext.ApplicationEvents);
            DbContext.PurgeTable<Core.Entities.Application, int>(DbContext.Applications);
            DbContext.SaveChangesAsync().Wait();
        }
    }
}
