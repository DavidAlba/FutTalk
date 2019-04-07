using System.Collections.Generic;
using WebMVC.Models.Pagination;

namespace WebMVC.Models.ViewModels
{
    public class MessagesListViewModel
    {
        public IEnumerable<Message> Messages { get; set; }
        public PagingInfo<Message> PagingInfo { get; set; }
        public string CurrentCategory { get; set; }
    }
}