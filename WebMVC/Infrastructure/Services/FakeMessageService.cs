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
                        Category = string.Format("Cat{0}", (i % 2 == 0) ? "1" : "2")
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

        public PagingInfo<Message> GetAllMessages(string category, int size = 5, int index = 1)
        {
            IEnumerable<Message> messages = null;
            var longCount = _messages.LongCount();

            if (longCount > 0L)
                messages = _messages.AsQueryable<Message>()
                   .Where(m => category == null || m.Category == category)
                   .OrderBy(m => m.Id)
                   .Skip((index - 1) * size)
                   .Take(size)
                   .AsEnumerable<Message>();            

            return new PagingInfo<Message> { CurrentPage = index, ItemsPerPage = size, TotalItems = longCount, Items = messages };
        }
        

        public Task<PagingInfo<Message>> GetAllMessagesAsync(string category, int size = 5, int index = 1)
        {        
            return Task<IEnumerable<Message>>.Run(() => {
                return this.GetAllMessages(category, size, index);
            });
        }

        public Message GetMessageById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Message> GetMessageByIdAsync(int id)
        {
            throw new NotImplementedException();
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
    }
}
