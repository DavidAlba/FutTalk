using Capsule.API.Infrastructure.Exceptions;
using Capsule.API.Infrastructure.Extensions;
using Capsule.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Capsule.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CapsuleController : Controller
    {
        private ICapsuleRepository _repository;

        public CapsuleController(ICapsuleRepository repository)
        {            
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{id}")]        
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Models.Capsule), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Models.Capsule), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCapsuleByUser(string user)
        {
            IActionResult result = null;
            Models.Capsule capsule = null;

            try
            {
                if (string.IsNullOrEmpty(user)) return BadRequest($"The capsule is invalid (null or empty string)");

                capsule = await _repository.GetCapsuleByUserAsync(user);
                if (capsule != null)
                {
                    result = Ok(capsule);
                }
                else
                {                    
                    result = this.CreatedAtRoute(
                        routeName: "default",                        
                        value: await _repository.CreateNewCapsuleAsync(user));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new CapsuleDomainException("An error caused an exception", ex);
            }
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Models.Capsule), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Models.Capsule), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> SaveCapsule([FromBody]Models.Capsule capsule)
        {
            IActionResult result = null;
            Models.Capsule capsuleFromRepository = null;

            if (capsule == null) return BadRequest($"The capsule is invalid (null)");
            if (string.IsNullOrEmpty(capsule?.Id)) return BadRequest($"The capsule id is invalid (null or empty string)");

            try
            {            
                capsuleFromRepository = await _repository.GetCapsuleByUserAsync(capsule.Id);
                if (capsuleFromRepository != null)
                {
                    result = Ok(await _repository.SaveCapsuleAsync(capsule));
                }
                else
                {
                    result = this.CreatedAtRoute(
                            routeName: "default",
                            value: await _repository.CreateNewCapsuleAsync(capsule));
                }

                return result;

            }
            catch (Exception ex)
            {
                throw new CapsuleDomainException("An error caused an exception", ex);
    }
}
    }
}