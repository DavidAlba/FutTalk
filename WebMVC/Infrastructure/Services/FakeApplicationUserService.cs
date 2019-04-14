using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Infrastructure.Services
{
    public class FakeApplicationUserService : IApplicationUserService
    {
        public string GetApplicaitonUser()
        {
            return "davidgalba@gmail.com";
        }
    }
}
