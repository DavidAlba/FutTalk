using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Catalog.API.Infrastructure.Extensions;

namespace Catalog.API
{
    public class Startup
    {        
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, IHostingEnvironment environment, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomSwagger(_environment, _logger)
                .AddCustomContextDatabase(_configuration, _environment, _logger)
                .AddCustomRepository(_environment, _logger)
                .AddCustomMVC(_environment, _logger);
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var pathBase = _configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                _logger.LogDebug("Using PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }

            app.UseStaticFiles();
            app.UseMvc( routes =>
                {
                    routes.MapRoute(
                        name: "Default",
                        template: "{controller=Home}/{Action=Swagger}");
                }
            );

            app.UseCors("CorsPolicy");
            app.UseSwagger()
              .UseSwaggerUI(c =>
              {
                  c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", $"{env.ApplicationName} V1");
              });
        }
    }
}
