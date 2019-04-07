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
        private IMessageService _service;
        public int PageSize = 2;

        public MessageController(IMessageService service) => _service = service;

        public async Task<IActionResult> List(string category, int page = 1)
        {
            var pagingInfo = await _service.GetAllMessagesAsync(
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