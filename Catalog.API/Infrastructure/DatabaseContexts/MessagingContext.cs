using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Catalog.API.Infrastructure.DatabaseContexts
{
    public class MessagingContext : DbContext
    {
        public MessagingContext(DbContextOptions<MessagingContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Fluent API
            //builder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
        }
    }

    public class MessagingContextDesignFactory : IDesignTimeDbContextFactory<MessagingContext>
    {
        public MessagingContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=FutTalk.MessagesDataBase;Trusted_Connection=True;";
            var optionsBuilder = new DbContextOptionsBuilder<MessagingContext>()
                .UseSqlServer(
                    connectionString, 
                    options => options.EnableRetryOnFailure())
                .ConfigureWarnings(warning => warning.Throw(RelationalEventId.QueryClientEvaluationWarning));

            return new MessagingContext(optionsBuilder.Options);
        }
    }
}
