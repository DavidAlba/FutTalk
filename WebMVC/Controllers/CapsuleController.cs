using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;
using WebMVC.Models.ViewModels;

namespace WebMVC.Controllers
{
    public class CapsuleController : Controller
    {
        private IMessageService _messageService;
        private ICapsuleService _capsuleService;

        public CapsuleController(IMessageService messageService, ICapsuleService capsuleService)
        {
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _capsuleService = capsuleService ?? throw new ArgumentNullException(nameof(capsuleService));
        }

        public async Task<ViewResult> Index(string returnUrl)
        {
            ViewResult result = View();

            try
            {
                result = View(
                    new CapsuleIndexViewModel
                    {
                        Capsule = await _capsuleService.GetCapsuleByUserAsync(),
                        ReturnUrl = returnUrl
                    }
                 );
            }
            catch (Exception) // (BrokenCircuitException)
            {
                // Catch error when API is in circuit-opened mode                 
                //HandleBrokenCircuitException();                
                
            }

            return result;
        }

        public async Task<IActionResult> AddMessageToCapsule(int messageId, string returnUrl)
        {
            IActionResult result = RedirectToAction("Index", "Capsule", new { returnUrl });

            try
            { 
                var message = await _messageService.GetMessageByIdAsync(messageId);
                if (message != null)
                {
                    Models.Capsule capsule = await _capsuleService.GetCapsuleByUserAsync();                    
                    capsule?.AddMessage(message);
                    await _capsuleService.SaveCapsuleAsync(capsule: capsule);
                }
            }
            catch (Exception) // (BrokenCircuitException)
            {
                // Catch error when API is in circuit-opened mode                 
                //HandleBrokenCircuitException();
            }

            return result;
        }

        public async Task<IActionResult> RemoveFromCapsule(int messageId, string returnUrl)
        {
            IActionResult result = RedirectToAction("Index", "Capsule", new { returnUrl });

            try
            {
                Models.Capsule capsule = await _capsuleService.GetCapsuleByUserAsync();
                Message message = capsule[messageId];
                if (message != null)
                {
                    capsule.RemoveMessage(message);
                    capsule = await _capsuleService.SaveCapsuleAsync(capsule);
                }
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