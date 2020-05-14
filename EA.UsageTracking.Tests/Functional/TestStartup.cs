using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ardalis.ListStartupServices;
using AutoMapper;
using EA.UsageTracking.Application.API;
using EA.UsageTracking.Application.API.Authorization;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Behaviors;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Functional;
using EA.UsageTracking.Tests.Integration;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Moq;
using StackExchange.Redis;
using Steeltoe.CloudFoundry.Connector.Redis;

namespace EA.UsageTracking.Tests.Functional
{
    public class TestStartup: Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureAuthentication(IServiceCollection services)
        {
            var issuer = Configuration["issuer"];
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = issuer;
                options.TokenValidationParameters = new TokenValidationParameters() { ValidateAudience = false };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Policy.UsageUser, policy => policy.Requirements.Add(new HasScopeRequirement(Constants.Policy.UsageUser, issuer)));
                options.AddPolicy(Constants.Policy.UsageApp, policy => policy.Requirements.Add(new HasScopeRequirement(Constants.Policy.UsageApp, issuer)));
                options.AddPolicy(Constants.Policy.UsageAdmin, policy => policy.Requirements.Add(new HasScopeRequirement(Constants.Policy.UsageAdmin, issuer)));
            });

            services.AddSingleton<IAuthorizationHandler, HasScopeTestHandler>();

        }

        protected override void ConfigureRedis(IServiceCollection services)
        {
            var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
            mockConnectionMultiplexer.Setup(x => x.GetSubscriber(null)).Returns(new FakeSubscriber());
            services.AddSingleton(mockConnectionMultiplexer.Object);
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
            app.UseAuthorization();

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
