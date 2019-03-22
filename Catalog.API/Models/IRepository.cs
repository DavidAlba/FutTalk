using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Models
{  
    public interface IRepository
    {       
        IEnumerable<Message> GetAllMessages();
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Message GetMessageById(int id);
        Task<Message> GetMessageByIdAsync(int id);
        Message AddMessage(Message message);
        Task<Message> AddMessageAsync(Message message);
        Message UpdateMessage(int messageId, Message message);
        Task<Message> UpdateMessageAsync(int messageId, Message message);
        Message ReplaceMessage(Message message);
        Task<Message> ReplaceMessageAsync(Message message);
        void RemoveMessage(int id);
        Task RemoveMessageAsync(int id);
    }
}