using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Models
{
    public class Message
    {
        public int Id { get; set;  }
        public string Name { get; set; }
        public string Body { get; set; }
    }
}
