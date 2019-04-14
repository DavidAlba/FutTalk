using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Infrastructure.Services;

namespace WebMVC.Infrastructure.ViewComponents
{
    public class MessageNavigationMenuViewComponent : ViewComponent
    {
        private IMessageService _service = null;

        public MessageNavigationMenuViewComponent(IMessageService service) => _service = service;

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(
                 _service.GetAllMessages()
                    .Select(m => m.Category)
                    .Distinct()
                    .OrderBy(m => m)
                );
        }
    }
}
