using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class DeliveryController : Controller
    {
        private IDeliveryService _deliveryService;
        private ICapsuleService _capsuleService;

        public DeliveryController(IDeliveryService deliveryService, ICapsuleService capsuleService)
        {
            _deliveryService = deliveryService ?? throw new ArgumentNullException(nameof(deliveryService));
            _capsuleService = capsuleService ?? throw new ArgumentNullException(nameof(capsuleService));
        }

        public async Task<IActionResult> Index()
        {
            IActionResult result = null;

            try
            {
                var capsule = await _capsuleService.GetCapsuleByUserAsync();
                if (capsule != null && !capsule.AnyMessage())
                    ModelState.AddModelError("Capsule", "Sorry, your capsule is empty!");

                result = View(
                    viewName: nameof(Index),
                    model: new Delivery(capsule)
                    );
            }
            catch (Exception) // (BrokenCircuitException)
            {
                // Catch error when API is in circuit-opened mode                 
                //HandleBrokenCircuitException();
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> SendCapsule(Delivery delivery)
        {
            IActionResult result = null;

            try
            {
                if (ModelState.IsValid)
                {
                    delivery.Capsule = await _capsuleService.GetCapsuleByUserAsync();
                    await _deliveryService.DeliverCapsuleAsync(delivery);
                    result = RedirectToAction(
                        controllerName: "Delivery",
                        actionName: nameof(Success)
                        );
                }
                else
                {
                    result = View(
                        viewName: nameof(Index), 
                        model: delivery
                        );
                }
            }
            catch (Exception) // (BrokenCircuitException)
            {
                // Catch error when API is in circuit-opened mode                 
                //HandleBrokenCircuitException();                
                
            }

            return result;
        }

        public async Task<ViewResult> Success()
        {
            await _capsuleService.ClearAsync();
            return View();
        }
    }
}