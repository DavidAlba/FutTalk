using Microsoft.AspNetCore.Mvc;

namespace Capsule.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Swagger()
        {
            return this.LocalRedirect("~/swagger");
        }
    }
}