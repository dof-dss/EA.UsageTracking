using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ardalis.ListStartupServices;
using AutoMapper;
using EA.UsageTracking.Application.API.ActionFilters;
using EA.UsageTracking.Application.API.Authorization;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure;
using EA.UsageTracking.Infrastructure.Behaviors;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using Steeltoe.CloudFoundry.Connector.Redis;

namespace EA.UsageTracking.Application.API
{
    public class Startup
    {
        private ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation("Issuer1: " + Environment.GetEnvironmentVariable("issuer"));
            _logger.LogInformation("Issuer2: " + Configuration["issuer"]);
            _logger.LogInformation("Issuer3: " + Configuration["VAR:issuer"]);

            services.AddControllers();
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/robotstxt", "/Robots.Txt");
                });

            services.AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;
                });

            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });

            services.AddAutoMapper(typeof(UsageTrackingContext).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly, 
                typeof(UsageTrackingContext).GetTypeInfo().Assembly,
                typeof(Result).GetTypeInfo().Assembly,
                typeof(UsageItem).GetTypeInfo().Assembly);

            ConfigureUsageTrackingContext(services);
            ConfigureSwagger(services);
            ConfigureServiceDescription(services);
            ConfigureAuthentication(services);
            ConfigureRedis(services);

            services.AddSingleton<IUriService, UriService>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
            services.AddTransient<IUsageTrackingContextFactory, UsageTrackingContextFactory>();
        }

        protected virtual void ConfigureRedis(IServiceCollection services)
        {
            services.AddDistributedRedisCache(Configuration);
            services.AddRedisConnectionMultiplexer(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, IDistributedCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseShowAllServicesMiddleware();
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<UsageTrackingContext>();
                context.Database.Migrate();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //Disable/refine in Production
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Usage Tracking API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization();
                endpoints.MapRazorPages();
            });
        }

        protected void ConfigureUsageTrackingContext(IServiceCollection services)
        {
            services.AddDbContext<UsageTrackingContext>(options => options.UseMySql(Configuration));

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton(sp =>
            {
                var builder = new DbContextOptionsBuilder<UsageTrackingContext>();
                return builder.UseMySql(Configuration).Options;
            });
        }

        protected virtual void ConfigureAuthentication(IServiceCollection services)
        {
            var issuer = Environment.GetEnvironmentVariable("issuer");
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

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

        }

        protected void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Usage Tracking API", Version = "v1" });
                c.OperationFilter<IgnoreParameterFilter>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
        }

        protected void ConfigureServiceDescription(IServiceCollection services)
        {
            services.Configure<ServiceConfig>(config =>
            {
                config.Services = new List<ServiceDescriptor>(services);
                config.Path = "/listservices";
            });
        }
    }
}
