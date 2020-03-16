using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Behaviors;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace EA.UsageTracking.Infrastructure
{
    public static class ContainerSetup
    {
        public static IServiceProvider InitializeWeb(Assembly webAssembly, IServiceCollection services) =>
            new AutofacServiceProvider(BaseAutofacInitialization(setupAction =>
            {
                setupAction.Populate(services);
                setupAction.RegisterAssemblyTypes(webAssembly).AsImplementedInterfaces();
            }));

        public static IContainer BaseAutofacInitialization(Action<ContainerBuilder> setupAction = null)
        {
            var builder = new ContainerBuilder();

            var coreAssembly = Assembly.GetAssembly(typeof(UsageItem));
            var infrastructureAssembly = Assembly.GetAssembly(typeof(UsageTrackingContext));
            var sharedKernelAssembly = Assembly.GetAssembly(typeof(Constants));

            //builder.RegisterType<AddUsageItemCommandHandler>().As<IRequestHandler<AddUsageItemCommand, Result<int>>>();
            //builder.RegisterDecorator<AddUsageItemCommandHandlerDecorator, IRequestHandler<AddUsageItemCommand, Result<int>>>();

            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();
            builder.RegisterType<UsageTrackingContextFactory>().As<IUsageTrackingContextFactory>().InstancePerLifetimeScope();
            builder.RegisterType<UriService>().As<IUriService>().SingleInstance();

            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ExceptionBehavior<,>)).As(typeof(IPipelineBehavior<,>));

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

            setupAction?.Invoke(builder);
            return builder.Build();
        }
    }
}
