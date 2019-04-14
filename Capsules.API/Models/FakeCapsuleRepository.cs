using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capsule.API.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace Capsule.API.Models
{
    public class FakeCapsuleRepository : ICapsuleRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor = null;

        public FakeCapsuleRepository(IHttpContextAccessor httpContextAccessor) 
            => _httpContextAccessor = httpContextAccessor;

        public Capsule CreateNewCapsule(string user) 
            => CreateNewCapsuleAsync(user).Result;

        public async Task<Capsule> CreateNewCapsuleAsync(string user)
            => await Task<Models.Capsule>.Run(() => {
                    _httpContextAccessor.HttpContext.Session.SetJson<Models.Capsule>(
                        user,
                        new Models.Capsule(user)
                        );
                    return GetCapsuleByUserAsync(user);
                }
            );

        public Capsule CreateNewCapsule(Capsule capsule) 
            => CreateNewCapsuleAsync(capsule).Result;

        public Task<Capsule> CreateNewCapsuleAsync(Capsule capsule)
            => Task<Models.Capsule>.Run(() => _httpContextAccessor.HttpContext.Session.SetJson<Models.Capsule>(capsule.Id, capsule));

        public Capsule GetCapsuleByUser(string user) 
            => GetCapsuleByUserAsync(user).Result;

        public Task<Capsule> GetCapsuleByUserAsync(string user)
            => Task<Models.Capsule>.Run(() => _httpContextAccessor.HttpContext.Session.GetJson<Models.Capsule>(user));

        public Capsule SaveCapsule(Capsule capsule) => SaveCapsuleAsync(capsule).Result;

        public async Task<Capsule> SaveCapsuleAsync(Capsule capsule)
            => await Task<Models.Capsule>.Run(() => {
                _httpContextAccessor.HttpContext.Session.SetJson<Models.Capsule>(
                       capsule.Id,
                       capsule);
                return GetCapsuleByUserAsync(capsule.Id);
            });        
    }
}
