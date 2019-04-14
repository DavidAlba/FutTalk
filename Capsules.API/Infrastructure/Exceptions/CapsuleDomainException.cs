using System;

namespace Capsule.API.Infrastructure.Exceptions
{
    public class CapsuleDomainException : ApplicationException
    {
        public CapsuleDomainException(): base() {}

        public CapsuleDomainException(string message) : base(message) { }

        public CapsuleDomainException(string message, Exception ex): base(message, ex) { }
    }
}