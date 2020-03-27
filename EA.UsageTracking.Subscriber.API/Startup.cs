using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Behaviors;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.Redis;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;

namespace EA.UsageTracking.Subscriber.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<UsageSubscriberWorker>();
            services.AddControllers();

            Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = (o) =>
                o.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), null);
            services.AddDbContext<UsageTrackingContext>(options => options.UseMySql(Configuration, mySqlOptionsAction));

            services.AddAutoMapper(typeof(UsageTrackingContext).GetTypeInfo().Assembly);

            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly,
                typeof(UsageTrackingContext).GetTypeInfo().Assembly,
                typeof(Result).GetTypeInfo().Assembly,
                typeof(UsageItem).GetTypeInfo().Assembly);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddRedisConnectionMultiplexer(Configuration);

            services.AddSingleton<IUriService, UriService>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
            services.AddTransient<IUsageTrackingContextFactory, UsageTrackingContextFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
