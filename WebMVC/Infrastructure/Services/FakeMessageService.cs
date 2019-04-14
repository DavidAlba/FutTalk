using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Pagination;

namespace WebMVC.Infrastructure.Services
{
    public class FakeMessageService : IMessageService
    {
        private IEnumerable<Message> _messages;

        public FakeMessageService()
        {
            _messages = GetMessages();
        }

        private IEnumerable<Message> GetMessages()
        {
            for (int i = 0; i < 5; i++)
                yield return
                    new Message
                    {
                        Id = i + 1,
                        Name = $"Name {i + 1}",
                        Body = $"Body {i + 1}",
                        Category = string.Format("Message Cat{0}", (i % 2 == 0) ? "1" : "2")
                    };
        }

        public Message AddMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public Task<Message> AddMessageAsync(Message message)
        {
            throw new NotImplementedException();
        }

        public PagingInfo<Message> GetAllPaginatedMessages(string category, int size = int.MaxValue, int index = 1)
        {
            IEnumerable<Message> messages = new List<Message>().AsEnumerable<Message>(); // Empty list
            var longCount = _messages.AsQueryable<Message>()
                 .Where(m => category == null || m.Category == category)
                 .LongCount();

            if (longCount > 0L)
                messages = _messages.AsQueryable<Message>()
                   .Where(m => category == null || m.Category == category)
                   .OrderBy(m => m.Id)
                   .Skip((index - 1) * size)
                   .Take(size)
                   .AsEnumerable<Message>();            

            return new PagingInfo<Message> { CurrentPage = index, ItemsPerPage = size, TotalItems = longCount, Items = messages };
        }        

        public Task<PagingInfo<Message>> GetAllPaginatedMessagesAsync(string category, int size = int.MaxValue, int index = 1)
        {        
            return Task<IEnumerable<Message>>.Run(() => {
                return this.GetAllPaginatedMessages(category, size, index);
            });
        }

        public Message GetMessageById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Message> GetMessageByIdAsync(int id)
        {
            return Task<Message>.Run(() => _messages.AsEnumerable().FirstOrDefault(m => m.Id == id));
        }

        public void RemoveMessage(int id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveMessageAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Message ReplaceMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public Task<Message> ReplaceMessageAsync(Message message)
        {
            throw new NotImplementedException();
        }

        public Message UpdateMessage(int messageId, Message message)
        {
            throw new NotImplementedException();
        }

        public Task<Message> UpdateMessageAsync(int messageId, Message message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> GetAllMessages() 
            => GetAllMessagesAsync().Result;

        public Task<IEnumerable<Message>> GetAllMessagesAsync() 
            => Task<IEnumerable<Message>>.Run(() => GetAllPaginatedMessages(null).Items);
    }
}
