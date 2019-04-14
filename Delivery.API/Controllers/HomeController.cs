using Microsoft.AspNetCore.Mvc;

namespace Delivery.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Swagger()
        {
            return this.LocalRedirect("~/swagger");
        }
    }
}