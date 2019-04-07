using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Infrastructure.Services;

namespace WebMVC.Infrastructure.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddCustomHttpClientServices(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment, ILogger logger)
        {
            logger.LogInformation($"Addign HttpClientServices ({environment.ApplicationName})...");

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            if (environment.IsDevelopment())
            {                
                services.AddSingleton<IMessageService, FakeMessageService>();
            }
            else
            {
                //register delegating handlers
                //services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
                //services.AddTransient<HttpClientRequestIdDelegatingHandler>();

                //set 5 min as the lifetime for each HttpMessageHandler int the pool
                //services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(TimeSpan.FromMinutes(5));

                //add http client services
                services.AddHttpClient<IMessageService, MessageService>();
                //services.AddHttpClient<IMessageService, MessageService>()
                //    .AddPolicyHandler(
                //            HttpPolicyExtensions
                //            .HandleTransientHttpError()
                //            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                //                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                //    .AddPolicyHandler(
                //                HttpPolicyExtensions
                //                .HandleTransientHttpError()
                //                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

                //add custom application services
                //services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();
            }

            return services;
        }        

        public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment, ILogger logger)
        {
            logger.LogInformation($"Adding Custom MVC ({environment.ApplicationName})...");

            //services.AddOptions();
            //services.Configure<AppSettings>(configuration);

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSession();
            
            return services;
        }
    }
}
