using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class ClaimsController : Controller
    {
        //[Authorize]
        [Authorize(Policy = "DCUsers")]
        public ViewResult Index() => View(User?.Claims);
    }
}