using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;

namespace WebMVC.Infrastructure.Services
{
    public interface ICapsuleService
    {
        Models.Capsule GetCapsuleByUser();
        Task<Models.Capsule> GetCapsuleByUserAsync();
        Task<Models.Capsule> SaveCapsuleAsync(Models.Capsule capsule);
        Task ClearAsync();
    }
}
