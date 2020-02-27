using System;
using System.Collections.Generic;
using System.Reflection;
using EA.UsageTracking.Infrastructure;
using EA.UsageTracking.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using Ardalis.ListStartupServices;
using EA.UsageTracking.API.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EA.UsageTracking.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            ConfigureUsageTrackingContext(services);
            ConfigureSwagger(services);
            ConfigureServiceDescription(services);

            return ContainerSetup.InitializeWeb(Assembly.GetExecutingAssembly(), services);
        }

        private void ConfigureUsageTrackingContext(IServiceCollection services)
        {
            services.AddDbContext<UsageTrackingContext>(options => options.UseMySql(Configuration));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton(sp =>
            {
                var builder = new DbContextOptionsBuilder<UsageTrackingContext>();
                return builder.UseMySql(Configuration).Options;
            });
        }
        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Usage Tracking API", Version = "v1" });
                c.OperationFilter<TenantHeaderFilter>();
            });
        }

        private void ConfigureServiceDescription(IServiceCollection services)
        {
            services.Configure<ServiceConfig>(config =>
            {
                config.Services = new List<ServiceDescriptor>(services);
                config.Path = "/listservices";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseShowAllServicesMiddleware();
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<UsageTrackingContext>();
                if(!context.Database.ProviderName.Contains("Microsoft.EntityFrameworkCore.InMemory"))
                    context.Database.Migrate();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Usage Tracking API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
