using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Pagination;
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
            services.AddAutoMapper(Assembly.GetAssembly(typeof(UsageTrackingContext)));

            Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = (o) =>
                o.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(60), null);
            services.AddDbContext<UsageTrackingContext>(options => options.UseMySql(Configuration, mySqlOptionsAction));

            services.AddRedisConnectionMultiplexer(Configuration);
            services.AddTransient<IUsageTrackingContextFactory, UsageTrackingContextFactory>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUriService, UriService>();
            services.TryAddSingleton(sp =>
            {
                var builder = new DbContextOptionsBuilder<UsageTrackingContext>();
                return builder.UseMySql(Configuration).Options;
            });
            services.AddMediatR(typeof(Worker), typeof(UsageTrackingContext));
            services.AddHostedService<Worker>();
            services.AddControllers();
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
