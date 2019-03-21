using Catalog.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure.DatabaseContexts
{
    public class SqliteMessagingContextSeed
    {
        public SqliteMessagingContextSeed(SqliteMessagingContext context) => this.Context = context;

        public SqliteMessagingContext Context { get; }

        public void Seed()
        {
            if (!Context.Messages.Any())
            {
                List<Message> messages = GetMessages();
                messages.ForEach(x => Context.Messages.Add(x));
                Context.SaveChanges();
            }
        }

        public async Task SeedAsync()
        {
            if (!Context.Messages.Any())
            {   
                List<Message> messages = GetMessages();
                messages.ForEach(x => Context.Messages.Add(x));              
                await Context.SaveChangesAsync();
            }
        }

        private List<Message> GetMessages()
        {
            List<Message> messages = new List<Message>();
            for (int i = 0; i < 5; i++)
                messages.Add(
                    new Message
                    {
                        Id = i + 1,
                        Name = $"Name {i + 1}",
                        Body = $"Body {i + 1}"
                    });
            return messages;
        }
    }
}
