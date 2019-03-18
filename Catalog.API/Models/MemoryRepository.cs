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

        public Message GetMessage(int id) => Messages.SingleOrDefault<Message>((m) => m.Id == id);

        public Message AddMessage(Message message)
        {
            Messages.ToList<Message>().Add(message);
            return GetMessage(message.Id);
        }

        public Message ReplaceMessage(Message message)
        {
            DeleteMessage(message.Id);
            return AddMessage(message);
        }

        public Message UpdateMessage(Message message)
        {
            Message msg = GetMessage(message.Id);
            if (msg != null)
            {
                msg.Name = message.Name;
                msg.Body = message.Body;
            };

            return msg;
        }

        public void DeleteMessage(int id)
        {
            Messages.ToList<Message>().Remove(GetMessage(id));
        }
    }
}
