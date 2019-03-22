using Catalog.API.Infrastructure.DatabaseContexts;
using Catalog.API.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Catalog.API.Tests
{
    public class MessagingContextTests : IDisposable
    {
        public SqliteConnection Connection { get; private set; }
        public SqlServerMessagingContext Context { get; private set; }
        public IRepository Repository { get; private set; }

        public MessagingContextTests()
        {
            Connection = new SqliteConnection("DataSource =:memory: ");
            Connection.Open();

            Context = new SqlServerMessagingContext(
                    new DbContextOptionsBuilder<SqlServerMessagingContext>()
                        .UseSqlite(Connection)
                        .Options);

            Context.Database.EnsureDeleted();
            Context.Database.Migrate();

            Repository = new MessagingRepository(Context);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void AddMessageBadRequestMessageIsNull(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            // Act
            Message messageAdded = Repository.AddMessage(null);

            // Assert   
            Assert.Null(messageAdded);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void AddMessageBadRequestMessageIdIsLessThanOne(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            int messageIdToAdd = 1;
            Message messageToAdd =
                new Message()
                {
                    Id = messageIdToAdd,
                    Name = $"{messageIdToAdd} added",
                    Body = $"{messageIdToAdd} added"
                };

            // Act
            Message messageAdded = Repository.AddMessage(messageToAdd);

            // Assert   
            Assert.Null(messageAdded);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void AddMessageBadRequestMessageIdAlreadyExisting(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            int messageIdToAdd = 2;
            Message messageToAdd =
                new Message()
                {
                    Id = messageIdToAdd,
                    Name = $"{messageIdToAdd} added",
                    Body = $"{messageIdToAdd} added"
                };

            // Act
            Message messageAdded = Repository.AddMessage(messageToAdd);

            // Assert   
            Assert.Null(messageAdded);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void AddMessageOk(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            int messageIdToAdd = int.MaxValue;
            Message messageToAdd =
                new Message()
                {
                    Id = messageIdToAdd,
                    Name = $"{messageIdToAdd} added",
                    Body = $"{messageIdToAdd} added"
                };

            // Act
            Message messageAdded = Repository.AddMessage(messageToAdd);

            // Assert   
            Assert.NotNull(messageAdded);
            Assert.Equal(messageToAdd, messageAdded, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void ReplaceMessageBadRequestMessageIdIsLessThanOne(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            int messageIdReplaced = int.MinValue;
            Message messageToReplace =
                new Message
                {
                    Id = messageIdReplaced,
                    Name = $"Name {messageIdReplaced} replaced",
                    Body = $"Body {messageIdReplaced} replaced"
                };

            // Act
            Message messageReplaced = Repository.ReplaceMessage(null);

            // Assert
            Assert.Null(messageReplaced);
            Assert.Equal(messages, Repository.GetAllMessages(), Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void ReplaceMessageBadRequestMessageIsNUll(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            // Act
            Message messageReplaced = Repository.ReplaceMessage(null);

            // Assert
            Assert.Null(messageReplaced);
            Assert.Equal(messages, Repository.GetAllMessages(), Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void ReplaceMessageBadRequestNotFound(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            int messageIdReplaced = int.MaxValue;
            Message messageToReplace =
                new Message
                {
                    Id = messageIdReplaced,
                    Name = $"Name {messageIdReplaced} replaced",
                    Body = $"Body {messageIdReplaced} replaced"
                };

            // Act
            Message messageReplaced = Repository.ReplaceMessage(messageToReplace);

            // Assert
            Assert.Null(messageReplaced);
            Assert.Equal(messages, Repository.GetAllMessages(), Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void ReplaceMessageOk(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            int messageIdReplaced = 2;
            Message messageToReplace = 
                new Message {
                    Id = messageIdReplaced,
                    Name = $"Name {messageIdReplaced} replaced",
                    Body = $"Body {messageIdReplaced} replaced"
                };

            // Act
            Message messageReplaced = Repository.ReplaceMessage(messageToReplace);

            // Assert            
            Assert.Equal(messageToReplace, messageReplaced, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void UpdateMessageBadRequestMessageIdLessThanOne(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            int messageIdToUpdate = int.MinValue;
            Message messageToUpdate =
                new Message()
                {
                    Id = 2,
                    Name = $"Name {messageIdToUpdate} updated",
                    Body = $"Body {messageIdToUpdate} updated"
                };

            // Act
            Message messageUpdated = Repository.UpdateMessage(messageIdToUpdate, messageToUpdate);

            // Assert
            Assert.Null(messageUpdated);
            Assert.Equal(messages, Repository.GetAllMessages(), Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void UpdateMessageBadRequestMessageIsNull(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            int messageIdToUpdate = 2;          

            // Act
            Message messageUpdated = Repository.UpdateMessage(messageIdToUpdate, null);

            // Assert
            Assert.Null(messageUpdated);
            Assert.Equal(messages, Repository.GetAllMessages(), Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void UpdateMessageBadRequestMessageNotFound(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            int messageIdToUpdate = int.MaxValue;
            Message messageToUpdate =
                new Message()
                {
                    Id = messageIdToUpdate,
                    Name = $"Name {messageIdToUpdate} updated",
                    Body = $"Body {messageIdToUpdate} updated"
                };

            // Act
            Message messageUpdated = Repository.UpdateMessage(messageIdToUpdate, messageToUpdate);

            // Assert
            Assert.Null(messageUpdated);
            Assert.Equal(messages, Repository.GetAllMessages(), Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void UpdateMessageOk(Message[] messages)
        {            
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );

            int messageIdToUpdate = 2;
            Message messageToUpdate = Repository.GetMessageById(messageIdToUpdate);
            messageToUpdate.Name = $"{messageToUpdate.Name} updated";
            messageToUpdate.Body = $"{messageToUpdate.Body} updated";

            // Act
            Message messageUpdated = Repository.UpdateMessage(messageIdToUpdate, messageToUpdate);

            // Assert   
            Assert.NotNull(messageUpdated);
            Assert.Equal(messageToUpdate, messageUpdated, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void RemoveMessageOk(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );
            int messageIdToRemove = 3;
            Message messageToRemove = Repository.GetMessageById(messageIdToRemove);

            // Act
            Repository.RemoveMessage(messageIdToRemove);

            // Assert            
            Assert.Null(Repository.GetMessageById(messageIdToRemove));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void RemoveMessageBadRequestMessageNotFound(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
                );
            int messageIdToRemove = int.MaxValue;
            Message messageToRemove = Repository.GetMessageById(messageIdToRemove);

            // Act
            Repository.RemoveMessage(messageIdToRemove);

            // Assert            
            Assert.Null(Repository.GetMessageById(messageIdToRemove));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void GetAllMessagesOk(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
            );

            // Act
            IEnumerable<Message> messagesFromRepository = Repository.GetAllMessages();

            // Assert            
            Assert.Equal(messages, messagesFromRepository, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Fact]
        public void GetAllMessagesNoContent()
        {
            // Arrange & Act
            IEnumerable<Message> messagesFromRepository = Repository.GetAllMessages();

            // Assert            
            Assert.Empty(messagesFromRepository);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void GetMessageByIdOk(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
            );

            // Act
            Message messageFromRepository = Repository.GetMessageById(messages[0].Id);

            // Assert            
            Assert.Equal(messages[0], messageFromRepository, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void GetMessageByIdNoContent(Message[] messages)
        {
            // Arrange
            foreach (Message message in messages)
                Repository.AddMessage(
                    new Message
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Body = message.Body
                    }
            );

            // Act
            Message messageFromRepository = Repository.GetMessageById(int.MaxValue);

            // Assert            
            Assert.Null(messageFromRepository);
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
