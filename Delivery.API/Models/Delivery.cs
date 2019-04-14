using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.API.Models
{
    public class Delivery
    {
        public string Id { get; set; }
        public Capsule Capsule { get; set; }
        public long TimeStamp { get; set; }
        public DateTime Created { get; set; }
        public DateTime When { get; set; }        
        public string ToWhom { get; set; }
        public bool GiftWrap { get; set; }
        public bool Delivered { get; set; }
    }
}
