using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Infrastructure.Services;

namespace WebMVC.Infrastructure.ViewComponents
{
    public class PickUpNavigationMenuViewComponent : ViewComponent
    {
        private IDeliveryService _service = null;

        public PickUpNavigationMenuViewComponent(IDeliveryService service) => _service = service;

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(
                 _service.GetAllDeliveries("Delivery 1", DateTime.Now)
                    .Select(m => m.Category)
                    .Distinct()
                    .OrderBy(m => m)
                );
        }
    }
}
