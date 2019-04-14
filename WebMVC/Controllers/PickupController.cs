using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Infrastructure.Services;
using WebMVC.Models.ViewModels;

namespace WebMVC.Controllers
{
    public class PickUpController : Controller
    {
        public int PageSize = 2;
        private DateTime _when = DateTime.Now.AddDays(1);
        private string _toWhom = "Delivery 1";
        private IDeliveryService _deliveryService;

        public PickUpController(IDeliveryService deliveryService)
            => _deliveryService = deliveryService ?? throw new ArgumentNullException(nameof(deliveryService));

        public async Task<IActionResult> Index(string category, int page = 1)
        {
            var pagingInfo = await _deliveryService.GetAllPaginatedMessagesAsync(
               toWhom: _toWhom,
               when: _when,
               category: category,
               size: PageSize,
               index: page);

            return View(
                new PickupListViewModel()
                {
                    Deliveries = pagingInfo.Items,
                    PagingInfo = pagingInfo,
                    CurrentCategory = category
                }                
            );
        }

        public async Task<IActionResult> PickUpDelivery(string deliveryId, string returnUrl)
        {
            IActionResult result = null;

            try
            {
                var delivery = await _deliveryService.PickUpAsync(deliveryId);
                if (delivery != null)
                    result = View(delivery);
            }
            catch (Exception) // (BrokenCircuitException)
            {
                // Catch error when API is in circuit-opened mode                 
                //HandleBrokenCircuitException();
            }

            return result;
        }
    }
}