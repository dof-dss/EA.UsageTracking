using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Infrastructure.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steeltoe.Common.Hosting;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace EA.UsageTracking.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddEventLog();
                })
                .UseCloudHosting()
                .AddCloudFoundry()
                .UseStartup<Startup>();

    }
}
