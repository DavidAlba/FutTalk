using System;

namespace Catalog.API.Infrastructure.Exceptions
{
    public class FutTalkException : ApplicationException
    {
        public FutTalkException(): base() {}

        public FutTalkException(string message) : base(message) { }

        public FutTalkException(string message, Exception ex): base(message, ex) { }
    }
}