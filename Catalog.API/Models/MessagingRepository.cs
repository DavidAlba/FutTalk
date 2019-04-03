using Catalog.API.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Models
{
    public class MessagingRepository : IRepository
    {
        protected SqlServerMessagingContext _context = null;

        public MessagingRepository(SqlServerMessagingContext context)
        {            
            _context = context ?? throw new ArgumentNullException(nameof(context));
            ((DbContext)context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
        }       

        public IEnumerable<Message> GetAllMessages()
        {
            return GetAllMessagesAsync()?.Result;
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            return await _context.Messages.AsQueryable<Message>().ToListAsync();
        }

        public Message AddMessage(Message message) => AddMessageAsync(message)?.Result;

        public async Task<Message> AddMessageAsync(Message message)
        {
            Message result = null;
            Message messageFromDatabase = null;

            if (message == null || message?.Id <= 0) return null;

            messageFromDatabase = await GetMessageByIdAsync(message.Id);
            if (messageFromDatabase == null)
            {
                Message entity = _context.Messages.Add(
                     new Message
                     {
                         Id = message.Id,
                         Name = message.Name,
                         Body = message.Body
                     }
                 ).Entity;

                await _context.SaveChangesAsync();
                result = await GetMessageByIdAsync(message.Id);
            }

            return result;
        }

        public void RemoveMessage(int id) => RemoveMessageAsync(id)?.Wait();

        public async Task RemoveMessageAsync(int id)
        {
            Message message = await GetMessageByIdAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }

        public Message GetMessageById(int id) => GetMessageByIdAsync(id)?.Result;

        public async Task<Message> GetMessageByIdAsync(int id)
        {
            return await _context.Messages.SingleOrDefaultAsync(m => m.Id == id);
        }

        public Message ReplaceMessage(Message message) => ReplaceMessageAsync(message)?.Result;

        public async Task<Message> ReplaceMessageAsync(Message message)
        {
            Message messagetoDelete = null;
            Message result = null;

            if (message == null || message?.Id <= 0) return null;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    messagetoDelete = await GetMessageByIdAsync(message.Id);
                    if (messagetoDelete != null)
                    {
                        _context.Messages.Remove(messagetoDelete);
                        _context.Messages.Add(
                            new Message
                            {
                                Id = message.Id,
                                Name = message.Name,
                                Body = message.Body
                            }
                        );

                        await _context.SaveChangesAsync();
                        result = await GetMessageByIdAsync(message.Id);
                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }

                return result;
            }
        }

        public Message UpdateMessage(int messageId, Message message) => UpdateMessageAsync(messageId, message)?.Result;

        public async Task<Message> UpdateMessageAsync(int messageId, Message message)
        {
            Message messageToUpdate = null;

            if (messageId <= 0 || message == null) return null;

            messageToUpdate = await GetMessageByIdAsync(messageId);
            if (messageToUpdate != null)
            {
                messageToUpdate = message;
                _context.Messages.Update(messageToUpdate);
                await _context.SaveChangesAsync();
            }

            return await GetMessageByIdAsync(message.Id);
        }
    }
}
