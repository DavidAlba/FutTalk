using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Infrastructure.Extensions;
using WebMVC.Models;

namespace WebMVC.Infrastructure.Services
{
    public class FakeCapsuleService : ICapsuleService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = null;
        private readonly IApplicationUserService _applicationUserService = null;

        public FakeCapsuleService(IHttpContextAccessor httpContextAccessor, IApplicationUserService applicationUserService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _applicationUserService = applicationUserService ?? throw new ArgumentNullException(nameof(applicationUserService));
        }

        public Capsule GetCapsuleByUser()
            => GetCapsuleByUserAsync().Result;

        public async Task<Models.Capsule> GetCapsuleByUserAsync()
            => await Task<Models.Capsule>.Run(() => {
                var user = _applicationUserService.GetApplicaitonUser();
                Models.Capsule capsule = _httpContextAccessor.HttpContext.Session.GetJson<Models.Capsule>(user);
                if (capsule == null)
                {
                    capsule = _httpContextAccessor.HttpContext.Session.SetJson<Models.Capsule>(
                        user,
                        new Models.Capsule(user));
                }
                return capsule;
            });

        public async Task<Models.Capsule> SaveCapsuleAsync(Models.Capsule capsule)
            => await Task<Models.Capsule>.Run(() => {
                Models.Capsule capsuleFromSession =
                _httpContextAccessor.HttpContext.Session.SetJson<Models.Capsule>(
                    capsule.Id,
                    capsule);
                return capsuleFromSession;
            });

        public async Task ClearAsync()
            => await Task.Run(() => {
                var user = _applicationUserService.GetApplicaitonUser();
                Models.Capsule capsule = _httpContextAccessor.HttpContext.Session.GetJson<Models.Capsule>(user);
                if (capsule != null)
                    _httpContextAccessor.HttpContext.Session.SetJson<Models.Capsule>(
                        capsule?.Id,
                        null);
            });
    }
}