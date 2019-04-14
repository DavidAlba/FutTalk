using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capsule.API.Models
{
    public class Capsule
    {
        public Capsule(string userId) 
            => this.Id = userId;

        public string Id { get; set; }
        public IEnumerable<Message> Messages { get; set; } = new List<Message>();
    }
}
