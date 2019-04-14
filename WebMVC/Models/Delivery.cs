using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models
{
    public class Delivery
    {
        private DateTime _when;

        public Delivery(string deliveryId, Capsule capsule)
        {
            Id = deliveryId;
            Capsule = capsule;
            Created = DateTime.Now;
        }

        public Delivery(Capsule capsule) : this(Guid.NewGuid().ToString(), capsule) { }

        public Delivery(string deliveryId) : this(deliveryId, null) { }

        public Delivery() : this(Guid.NewGuid().ToString(), null) { }

        public string Id { get;  }
        public Capsule Capsule { get; set; }
        public long TimeStamp { get; }
        public DateTime Created { get; }

        [Required(ErrorMessage = "Please enter when you wish to capsule")]
        public DateTime When
        {
            get => _when;
            set
            {
                //if (value <= DateTime.Now) throw new ArgumentException("The date/time when you send your capsule must greater than now");
                //if (value < Capsule.Created) throw new ArgumentException("The date/time when you send your capsule must greater than the capsule creation date");
                _when = value;
            }
        }

        [Required(ErrorMessage = "Please enter to whom you wish to capsule")]
        public string ToWhom { get; set; }
        
        public bool GiftWrap { get; set; }
        public bool Delivered { get; set; }
        public string Category { get; set; }
    }
}
