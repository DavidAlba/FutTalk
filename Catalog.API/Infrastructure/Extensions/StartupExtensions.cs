using Catalog.API.Infrastructure.DatabaseContexts;
using Catalog.API.Infrastructure.Filters;
using Catalog.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddContextDatabase(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment, ILogger logger)
        {
            logger.LogInformation("Configuring database({ApplicationContext})...", environment.ApplicationName);

            if (environment.IsDevelopment())
            {
                // Register DbContext as a Singleton life time
                services.AddDbContext<SqlServerMessagingContext>(
                    options =>
                    {
                        // Create databa context
                        options.UseSqlite("DataSource =:memory: ");
                    },
                    ServiceLifetime.Singleton
                );
            }
            else
            {
                // Register DbContext as a Singleton life time
                services.AddDbContext<SqlServerMessagingContext>(
                    options =>
                    {
                        options.UseSqlServer(
                            // Configuring Connection String
                            configuration["ConnectionString"],
                            // Configuring Connection Resiliency
                            sqlOptions => sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null));

                        // Changing default behavior when client evaluation occurs to throw. // Default in EFCore would be to log warning when client evaluation is done.
                        options.ConfigureWarnings(warning => warning.Throw(RelationalEventId.QueryClientEvaluationWarning));
                    },
                    ServiceLifetime.Singleton
                );
            }

            return services;
        }

        public static IServiceCollection AddCustomRepository(this IServiceCollection services, IHostingEnvironment environment, ILogger logger)
        {
            logger.LogInformation("Registering IRepository dependency injection ({ApplicationContext})...", environment.ApplicationName);

            if (environment.IsDevelopment())
            {
                // Register repository as a Singleton life time 
                services.AddSingleton<IRepository, MessagingInMemoryRepository>();
            }
            else
            {
                // Register repository as a Singleton life time 
                services.AddSingleton<IRepository, MessagingRepository>();
            }

            return services;
        }

        public static IServiceCollection AddCustomMVC(this IServiceCollection services, IHostingEnvironment environment, ILogger logger)
        {

            logger.LogInformation("Adding Custom MVC ({ApplicationContext})...", environment.ApplicationName);

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

        public static IServiceCollection AddSwagger(this IServiceCollection services, IHostingEnvironment environment, ILogger logger)
        {
            logger.LogInformation("Adding Swagger ({ApplicationContext})...", environment.ApplicationName);
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc(
                    "v1",
                    new Swashbuckle.AspNetCore.Swagger.Info
                    {
                        Title = $"{environment.ApplicationName} Web API",
                        Version = "v1",
                        Description = $"The {environment.ApplicationName} Microservice HTTP API. This is a Data-Driven/CRUD microservice sample",
                        TermsOfService = "Terms Of Service"
                    });
            });

            return services;
        }
    }
}
