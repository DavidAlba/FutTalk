using System;

namespace Delivery.API.Infrastructure.Exceptions
{
    public class DeliveryDomainException : ApplicationException
    {
        public DeliveryDomainException(): base() {}

        public DeliveryDomainException(string message) : base(message) { }

        public DeliveryDomainException(string message, Exception ex): base(message, ex) { }
    }
}