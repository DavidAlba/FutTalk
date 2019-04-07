using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebMVC.Infrastructure.Extensions;

namespace WebMVC
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
            services.AddCustomHttpClientServices(_configuration, _environment, _logger)
                .AddCustomMVC(_configuration, _environment, _logger);
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            
            app.UseStaticFiles();
            app.UseMvc(routes => {

                // Shows the specified page of items from the specified category
                routes.MapRoute(
                    name: null,
                    template: "{category}/Page{page:int}",
                    defaults: new { controller = "Message", action = "List" }
                );

                // Shows the first page of items from a specific category
                routes.MapRoute(
                    name: null,
                    template: "Page{page:int}",
                    defaults: new { controller = "Message", action = "List", page = 1 }
                );


                // Lists the specified page, showing items from all categories
                routes.MapRoute(
                    name: null,
                    template: "{category}",
                    defaults: new { controller = "Message", action = "List", page = 1 }
                );

                // Lists the first page of messages from all categories
                routes.MapRoute(
                    name: null,
                    template: "",
                    defaults: new { controller = "Message", action = "List", page = 1}
                );

                // Default route
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Message}/{action=List}/{id?}"
                );
            });
        }
    }
}
