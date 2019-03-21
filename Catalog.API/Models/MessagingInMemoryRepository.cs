using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Models
{
    public class MessagingInMemoryRepository : IRepository
    {
        public MessagingInMemoryRepository()
        {
            Messages = 
                new List<Message> {
                    new Message { Id = 1, Name = "Name 1", Body = "Body 1" },
                    new Message { Id = 2, Name = "Name 2", Body = "Body 2" },
                    new Message { Id = 3, Name = "Name 3", Body = "Body 3" }
                };
        }

        public IEnumerable<Message> Messages { get; set; }

        public Message GetMessageById(int id) => Messages.SingleOrDefault<Message>((m) => m.Id == id);

        public Message AddMessage(Message message)
        {
            Messages.ToList<Message>().Add(message);
            return GetMessageById(message.Id);
        }
        public Message ReplaceMessage(Message message)
        {
            Message messageToDelete = null;
            Message messaFromMemory = null;
            Message result = null;
                
            if ( message == null || message?.Id <= 0) return null;

            messageToDelete = 
                new Message
                {
                    Id = message.Id,
                    Name = message.Name,
                    Body = message.Body
                };

            try
            {

                messaFromMemory = GetMessageById(message.Id);
                if (messaFromMemory != null)
                {
                    RemoveMessage(messaFromMemory.Id);
                    result = AddMessage(message);
                }                
            }
            catch (Exception)
            {
                // Compensatory action
                messaFromMemory = GetMessageById(message.Id);
                if (messaFromMemory == null)
                    AddMessage(messageToDelete);
            }

            return result;
        }

        public Message UpdateMessage(int messageId, Message message)
        {
            if (messageId <= 0 || message == null) return null;

            Message msg = GetMessageById(message.Id);
            if (msg != null)
            {
                msg.Name = message.Name;
                msg.Body = message.Body;
            };

            return msg;
        }

        public void RemoveMessage(int id)
        {
            Messages.ToList<Message>().Remove(GetMessageById(id));
        }
    }
}
