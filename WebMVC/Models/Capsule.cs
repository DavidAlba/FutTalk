using System;
using System.Collections.Generic;

namespace WebMVC.Models
{
    public class Capsule
    {
        private List<Message> _messages;

        public Capsule(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("The capsule Id cannot be empty or null");

            Id = id;
            TimeStamp = DateTimeOffset.UnixEpoch.ToUnixTimeSeconds();
            Created = DateTime.Now;
            _messages = new List<Message>();
        }

        public string Id { get; }
        public long TimeStamp { get; }
        public DateTime Created { get; }                
        public IEnumerable<Message> Messages { get => _messages; }

        public Message this[int messageId]
        {
            get => _messages.Find((m) => m.Id == messageId);
        }

        public void AddMessage(Message message)
        {
            if (message == null) return;
            if (_messages.Exists((m) => m.Id == message.Id)) return;

            _messages.Add(message);
        }
        public void RemoveMessage(Message message) 
            => _messages.Remove(message);
        public bool AnyMessage() 
            => _messages.Count != 0;
    }
}