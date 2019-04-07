using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swagger;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Pagination;

namespace WebMVC.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        public MessageService(HttpClient httpClient, ILogger<MessageService> logger) // IOptions<AppSettings> settings)
        {
            
        }

        public Message AddMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public Task<Message> AddMessageAsync(Message message)
        {
            throw new NotImplementedException();
        }

        public PagingInfo<Message> GetAllMessages(string category, int size = 5, int index = 0)
        {
            return null;            
        }

        public Task<PagingInfo<Message>> GetAllMessagesAsync(string catgory, int size = 5, int index = 0)
        {
            
            return null;
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
