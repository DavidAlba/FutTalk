﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Pagination;

namespace WebMVC.Infrastructure.Services
{
    public interface IMessageService
    {
        IEnumerable<Message> GetAllMessages();
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        PagingInfo<Message> GetAllPaginatedMessages(string category, int size = int.MaxValue, int index = 1);
        Task<PagingInfo<Message>> GetAllPaginatedMessagesAsync(string category, int size = int.MaxValue, int index = 1);
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
