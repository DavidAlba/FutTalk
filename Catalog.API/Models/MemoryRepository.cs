using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Models
{
    public class MemoryRepository : IRepository
    {
        public MemoryRepository()
        {
            Messages = 
                new List<Message> {
                    new Message { Id = 1, Name = "Name 1", Body = "Body 1" },
                    new Message { Id = 2, Name = "Name 2", Body = "Body 2" },
                    new Message { Id = 3, Name = "Name 3", Body = "Body 3" }
                };
        }

        public IEnumerable<Message> Messages { get; set; }

    }
}
