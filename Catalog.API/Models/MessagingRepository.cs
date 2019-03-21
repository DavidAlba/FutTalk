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
        private SqlServerMessagingContext _context = null;

        public MessagingRepository(SqlServerMessagingContext context)
        {            
            _context = context ?? throw new ArgumentNullException(nameof(context));
            ((DbContext)context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
        }

        public IEnumerable<Message> Messages {
            get => _context.Messages.ToList();
            set => throw new NotImplementedException();
        }

        public Message AddMessage(Message message)
        {
            Message result = null;
            Message messageFromDatabase = null;

            if (message == null || message?.Id <= 0) return null;

            messageFromDatabase = this.GetMessageById(message.Id);
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

                _context.SaveChanges();
                result = GetMessageById(message.Id);
            }

            return result;
        }

        public void RemoveMessage(int id)
        {
            Message message = this.GetMessageById(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                _context.SaveChanges();
            }
        }

        public Message GetMessageById(int id)
        {
            return _context.Messages.SingleOrDefault(m => m.Id == id);
        }

        public Message ReplaceMessage(Message message)
        {
            Message messagetoDelete = null;
            Message result = null;

            if(message == null || message?.Id <= 0) return null;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    messagetoDelete = this.GetMessageById(message.Id);
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

                        _context.SaveChanges();
                        result = GetMessageById(message.Id);
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

        public Message UpdateMessage(int messageId, Message message)
        {
            Message messageToUpdate = null;

            if (messageId <= 0 || message == null) return null;

            messageToUpdate = GetMessageById(messageId);
            if (messageToUpdate != null)
            {
                messageToUpdate = message;
                _context.Messages.Update(messageToUpdate);
                _context.SaveChanges();
            }
            
            return GetMessageById(message.Id);
        }
    }
}
