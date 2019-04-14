using System;

namespace Catalog.API.Infrastructure.Exceptions
{
    public class MessagesDomainException : ApplicationException
    {
        public MessagesDomainException(): base() {}

        public MessagesDomainException(string message) : base(message) { }

        public MessagesDomainException(string message, Exception ex): base(message, ex) { }
    }
}