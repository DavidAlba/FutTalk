﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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