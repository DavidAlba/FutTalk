using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    [Authorize(Roles = "Admins, Users")]
    public class HomeController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager)
            => _userManager = userManager;

        public IActionResult Index() 
            => View(GetData(nameof(Index)));

        private Dictionary<string, object> GetData(string actionName) 
            => new Dictionary<string, object>
                {
                    ["Action"] = actionName,
                    ["User"] = HttpContext.User.Identity.Name,
                    ["Authenticated"] = HttpContext.User.Identity.IsAuthenticated,
                    ["Auth Type"] = HttpContext.User.Identity.AuthenticationType,
                    ["In Users Role"] = HttpContext.User.IsInRole("Users"),
                    ["FirstName"] = CurrentUser.Result.FirstName,
                    ["LastName"] = CurrentUser.Result.LastName
            };

        [Authorize]
        public async Task<IActionResult> UserProps()
            => View(await CurrentUser);

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserProps([Required]string firstName, [Required]string lastName)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await CurrentUser;
                user.FirstName = firstName;
                user.LastName = lastName;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            return View(await CurrentUser);
        }

        private Task<ApplicationUser> CurrentUser 
            => _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
    }
}