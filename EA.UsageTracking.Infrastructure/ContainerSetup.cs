using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Behaviors;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
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
            var sharedKernelAssembly = Assembly.GetAssembly(typeof(BaseEntity));

            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            builder.RegisterAssemblyTypes(sharedKernelAssembly, coreAssembly, infrastructureAssembly).AsImplementedInterfaces();

            setupAction?.Invoke(builder);
            return builder.Build();
        }
    }
}
