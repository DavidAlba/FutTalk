using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class AdminController : Controller
    {
        private IMessageService _messageService;

        public AdminController(IMessageService messageService)
            => _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));

        public async Task<ViewResult> Index()
            => View(await _messageService.GetAllMessagesAsync());

        public async Task<ViewResult> EditMessage(int messageId)
            => View(await _messageService.GetMessageByIdAsync(messageId));

        [HttpPost]
        public async Task<IActionResult> EditMessage(Message message)
        {
            IActionResult result = null;

            if (ModelState.IsValid)
            {
                var msg = await _messageService.UpdateMessageAsync(message.Id, message);
                if (msg == null)
                    msg = await _messageService.AddMessageAsync(message);

                TempData["message"] = $"{msg.Name} has been saved";
                result = RedirectToAction(
                    actionName: nameof(Index));
            }
            else
            {
                result = View(message);
            }

            return result;
        }

        public ViewResult CreateMessage() 
            => View(
                 viewName: nameof(EditMessage),
                 model: new Message());

        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            Message deleteMessage = await _messageService.RemoveMessageAsync(messageId);
            if (deleteMessage != null)
                TempData["message"] = $"{deleteMessage.Name} was deleted";
            return RedirectToAction(
                    actionName: nameof(Index));
        }
    }
}