using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Pagination;

namespace WebMVC.Models.ViewModels
{
    public class PickupListViewModel
    {
        public IEnumerable<Delivery> Deliveries { get; set; }
        public PagingInfo<Delivery> PagingInfo { get; set; }
        public string CurrentCategory { get; set; }
    }
}
