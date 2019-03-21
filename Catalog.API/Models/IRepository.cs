using System.Collections.Generic;

namespace Catalog.API.Models
{  
    public interface IRepository
    {       
        IEnumerable<Message> Messages { get; set; }
        Message GetMessageById(int id);
        Message AddMessage(Message message);
        Message UpdateMessage(int messageId, Message message);
        Message ReplaceMessage(Message message);
        void RemoveMessage(int id);
    }
}