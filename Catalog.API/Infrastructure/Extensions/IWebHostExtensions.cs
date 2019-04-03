using Catalog.API.Infrastructure.DatabaseContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Catalog.API.Infrastructure.Extensions
{
    public static class IWebHostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    // In-memory database only exists while the connection is open
                    if (context.Database.IsSqlite() && 
                        context.Database.GetDbConnection().ConnectionString.Equals("DataSource =:memory: "))
                    {
                        logger.LogInformation($"The database provider {context.Database.ProviderName} is in-memory. Openning connection. In-memory database only exists while the connection is open");
                        context.Database.GetDbConnection().Open();
                    }

                    // Migrating database
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");
                    context.Database.EnsureDeleted();
                    context.Database.Migrate();
                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");

                    // Seeding database
                    seeder(context, services);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context { typeof(TContext).Name}");
                }
            }

            return webHost;
        }
    }
}
