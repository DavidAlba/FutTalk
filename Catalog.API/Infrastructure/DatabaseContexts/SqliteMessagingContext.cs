using Catalog.API.Infrastructure.EntityConfigurations;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Data.Sqlite;
using System;

namespace Catalog.API.Infrastructure.DatabaseContexts
{
    public class SqliteMessagingContext : DbContext
    {
        public SqliteMessagingContext(DbContextOptions<SqliteMessagingContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MessageEntityTypeConfiguration());
        }
    }

    public class SqliteMessagingContextDesignFactory : IDesignTimeDbContextFactory<SqliteMessagingContext>
    {
        public SqliteMessagingContext CreateDbContext(string[] args)
        {
            var connectionString = "Data Source=c:\\FutTalk.MessagesDataBase.db;";
            var optionsBuilder = 
                new DbContextOptionsBuilder<SqliteMessagingContext>().UseSqlite(connectionString);

            return new SqliteMessagingContext(optionsBuilder.Options);
        }
    }
}
