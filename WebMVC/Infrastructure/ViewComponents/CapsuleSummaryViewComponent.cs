using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;

namespace WebMVC.Infrastructure.ViewComponents
{
    public class CapsuleSummaryViewComponent : ViewComponent
    {
        private ICapsuleService _capsuleService;

        public CapsuleSummaryViewComponent(ICapsuleService capsuleService) 
            => _capsuleService = capsuleService;

        public IViewComponentResult Invoke()
            => View(_capsuleService.GetCapsuleByUser());

    }
}
