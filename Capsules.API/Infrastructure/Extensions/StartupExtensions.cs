using Capsule.API.Infrastructure.Filters;
using Capsule.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Capsule.API.Infrastructure.Extensions
{
    public static class StartupExtensions
    {       

        public static IServiceCollection AddCustomRepository(this IServiceCollection services, IHostingEnvironment environment, ILogger logger)
        {
            logger.LogInformation($"Registering {nameof(ICapsuleRepository)} dependency injection ({environment.ApplicationName})...");

            if (environment.IsDevelopment())
            {
                // Fake Capsule Repository uses Session
                services.AddHttpContextAccessor();
                services.AddMemoryCache();
                services.AddSession();

                // Register repository as a Singleton life time 
                services.AddSingleton<ICapsuleRepository, FakeCapsuleRepository>();
            }
            else
            {
                // Register repository as a Singleton life time 
                services.AddSingleton<ICapsuleRepository, CapsuleRepository>();
            }

            return services;
        }

        public static IServiceCollection AddCustomMVC(this IServiceCollection services, IHostingEnvironment environment, ILogger logger)
        {

            logger.LogInformation($"Adding Custom MVC ({environment.ApplicationName})...");

            services.AddMvc(options =>
                {
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();

            //services.AddApiVersioning(options =>
            //{                
            //    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            //});

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IHostingEnvironment environment, ILogger logger)
        {
            logger.LogInformation($"Adding Swagger ({environment.ApplicationName})...");
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc(
                    "v1",
                    new Swashbuckle.AspNetCore.Swagger.Info
                    {
                        Title = $"{environment.ApplicationName} Web API",
                        Version = "v1",
                        Description = $"The {environment.ApplicationName} Microservice HTTP API",
                        TermsOfService = "Terms Of Service"
                    });
            });

            return services;
        }
    }
}
