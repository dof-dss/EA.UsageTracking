using System;
using System.Collections.Generic;
using System.Text;
using Ardalis.ListStartupServices;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EA.UsageTracking.Tests.Functional
{
    public class TestStartup: Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime,
            IDistributedCache cache)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseShowAllServicesMiddleware();
            //    app.UseDeveloperExceptionPage();
            //}

            //app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseAuthentication();
            //app.UseAuthorization();

            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Usage Tracking API V1");
            //    c.RoutePrefix = string.Empty;
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
