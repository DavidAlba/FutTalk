using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Delivery.API
{
    public class Program
    {
        private static IHostingEnvironment _environment;
        private static ILogger<Program> _logger;

        public static int Main(string[] args)
        {
            try
            {
                IWebHost host = BuildWebHost(args);

                _logger = host.Services.GetService<ILogger<Program>>();
                _environment = host.Services.GetService<IHostingEnvironment>();
                _logger.LogInformation($"Webhost built ({_environment.ApplicationName})!");
                _logger.LogInformation($"The environment is ({_environment.EnvironmentName})!");
                _logger.LogInformation($"Running Webhost ({_environment.ApplicationName})!");
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex, $"Program terminated unexpectedly ({_environment.ApplicationName})!");
                return 1;
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return new WebHostBuilder()
                .CaptureStartupErrors(false)
                //.UseApplicationInsights()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) => {

                    var env = hostingContext.HostingEnvironment;
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    if (env.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                            config.AddUserSecrets(appAssembly, optional: true);
                    }

                    config.AddEnvironmentVariables();
                    if (args != null)
                        config.AddCommandLine(args);
                })
                .ConfigureLogging((hostingContext, logging) => {

                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) => {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
