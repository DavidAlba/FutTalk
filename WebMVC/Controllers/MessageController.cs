using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;
using WebMVC.Models.Pagination;
using WebMVC.Models.ViewModels;

namespace WebMVC.Controllers
{
    public class MessageController : Controller
    {
        private IMessageService _messageService;
        public int PageSize = 2;

        public MessageController(IMessageService messageService) 
            => _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));

        public async Task<IActionResult> Index(string category, int page = 1)
        {
            var pagingInfo = await _messageService.GetAllPaginatedMessagesAsync(
                category: category,
                size: PageSize, 
                index: page);

            return View(
                new MessagesListViewModel
                {
                    Messages = pagingInfo.Items,
                    PagingInfo = pagingInfo,
                    CurrentCategory = category
                });
        }            
    }
}