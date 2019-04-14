using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.API.Models
{
    public class Capsule
    {  
        public string Id { get; set;  }
        public long TimeStamp { get; set;  }
        public DateTime Created { get; set;  }
        public IEnumerable<ModelsDelivery> Messages { get; set; } = new List<ModelsDelivery>();
    }
}
