using System;

namespace Catalog.API.Infrastructure
{
    public class FutTalkException : ApplicationException
    {
        public FutTalkException(): base() {}
        public FutTalkException(string message, Exception ex): base(message, ex) { }
    }
}