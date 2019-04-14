using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WebMVC.Models;
using Xunit;

namespace WebMVC.UnitTests
{
    public class CapsuleTests
    {
        [Fact]
        public void CreateNewCapsule()
        {
            // Arrange
            string capsuleId = Guid.NewGuid().ToString();

            // Act
            Capsule capsule = new Capsule(capsuleId);

            // Assert
            Assert.Equal(capsuleId, capsule.Id);
            Assert.Equal(DateTime.Now, capsule.Created, Comparer.Get<DateTime>((d1, d2) => d1.Day == d2.Day && d1.Month == d2.Month && d1.Year == d2.Year));
            Assert.InRange(capsule.TimeStamp, DateTimeOffset.UnixEpoch.ToUnixTimeSeconds(), DateTimeOffset.UnixEpoch.ToUnixTimeSeconds() + 10);
            Assert.Empty(capsule.Messages);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void AddNewMessagesToCapsule(Message[] messages)
        {
            // Arrange
            Capsule capsule = new Capsule(Guid.NewGuid().ToString());

            // Act
            foreach (var msg in messages)
                capsule.AddMessage(msg);

            // Assert
            Assert.Equal(messages, capsule.Messages, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Fact]
        public void AddAnEmptyMessageToCapsule()
        {
            // Arrange
            Capsule capsule = new Capsule(Guid.NewGuid().ToString());

            // Act
            capsule.AddMessage(null);

            // Assert
            Assert.Empty(capsule.Messages);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void AddAnExistingMessageToCapsule(Message[] messages)
        {
            // Arrange           
            Capsule capsule = new Capsule(Guid.NewGuid().ToString());

            // Act
            capsule.AddMessage(messages[0]);
            capsule.AddMessage(messages[0]);

            // Assert
            Assert.Single(capsule.Messages);
        }


        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void RemoveMessageFromCapsule(Message[] messages)
        {
            // Arrange           
            Capsule capsule = new Capsule(Guid.NewGuid().ToString());
            foreach (var msg in messages)
                capsule.AddMessage(msg);

            // Act
            capsule.RemoveMessage(messages[1]);

            // Assert
            Assert.DoesNotContain(messages[1], capsule.Messages);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void AnyMessageTrue(Message[] messages)
        {
            // Arrange           
            Capsule capsule = new Capsule(Guid.NewGuid().ToString());

            // Act
            capsule.AddMessage(messages[0]);
            capsule.AddMessage(messages[0]);

            // Assert
            Assert.True(capsule.AnyMessage());
        }

        [Fact]
        public void AnyMessageFalse()
        {
            // Arrange           
            Capsule capsule = new Capsule(Guid.NewGuid().ToString());

            // Act & Assert
            Assert.False(capsule.AnyMessage());
        }
    }
}
