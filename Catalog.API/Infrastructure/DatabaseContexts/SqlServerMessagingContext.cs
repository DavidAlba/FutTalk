using Catalog.API.Infrastructure.EntityConfigurations;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

namespace Catalog.API.Infrastructure.DatabaseContexts
{
    public class SqlServerMessagingContext : DbContext
    {
        public SqlServerMessagingContext(DbContextOptions<SqlServerMessagingContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {            
            builder.ApplyConfiguration(new MessageEntityTypeConfiguration());
        }
    }

    public class SqlServerMessagingContextDesignFactory : IDesignTimeDbContextFactory<SqlServerMessagingContext>
    {
        public SqlServerMessagingContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=FutTalk.MessagesDataBase;Trusted_Connection=True;";
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerMessagingContext>()
                .UseSqlServer(
                    // Configuring Connection String
                    connectionString,
                    // Configuring Connection Resiliency
                    options => options.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null))

                // Changing default behavior when client evaluation occurs to throw. // Default in EFCore would be to log warning when client evaluation is done.
                .ConfigureWarnings(warning => warning.Throw(RelationalEventId.QueryClientEvaluationWarning));

            return new SqlServerMessagingContext(optionsBuilder.Options);
        }
    }
}
