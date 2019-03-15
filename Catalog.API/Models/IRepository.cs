using System.Collections.Generic;

namespace Catalog.API.Models
{
    public interface IRepository
    {
        IEnumerable<Message> Messages { get; set; }
    }
}