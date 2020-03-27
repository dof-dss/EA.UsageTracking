using System;
using System.Reflection;
using AutoMapper;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steeltoe.CloudFoundry.Connector.Redis;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Steeltoe.Common.Hosting;

namespace EA.UsageTracking.Subscriber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {                
                    services.AddHostedService<UsageSubscriberWorker>();
                    services.AddAutoMapper(Assembly.GetAssembly(typeof(UsageTrackingContext)));

                    Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = (o) =>
                        o.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), null);
                    services.AddDbContext<UsageTrackingContext>(options => options.UseMySql(hostContext.Configuration, mySqlOptionsAction));
                    
                    services.AddRedisConnectionMultiplexer(hostContext.Configuration);
                    services.AddTransient<IUsageTrackingContextFactory, UsageTrackingContextFactory>();
                    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                    services.AddSingleton<IUriService, UriService>();
                    services.TryAddSingleton(sp =>
                    {
                        var builder = new DbContextOptionsBuilder<UsageTrackingContext>();
                        return builder.UseMySql(hostContext.Configuration).Options;
                    });
                    services.AddMediatR(typeof(UsageSubscriberWorker), typeof(UsageTrackingContext));
                })
                .UseWindowsService()
                .AddCloudFoundry();
    }
}
