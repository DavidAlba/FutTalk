using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Swagger()
        {
            return this.LocalRedirect("~/swagger");
        }
    }
}