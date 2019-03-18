using System.Collections.Generic;

namespace Catalog.API.Models
{
    public interface IRepository
    {
        IEnumerable<Message> Messages { get; set; }
        Message GetMessage(int id);
        Message AddMessage(Message message);
        Message UpdateMessage(Message message);
        Message ReplaceMessage(Message message);
        void DeleteMessage(int id);
    }
}