using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Delivery.API.Infrastructure.Exceptions;
using Delivery.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : Controller
    {
        private IDeliveryRepository _repository;
    
        public DeliveryController(IDeliveryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Models.Delivery), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> SaveDelivery(Models.Delivery delivery)
        {
            try
            {
                IActionResult result = null;

                if (delivery == null) return BadRequest($"The delivery is invalid (null)");
                if (string.IsNullOrEmpty(delivery.Id)) return BadRequest($"The delivery id is invalid (null or empty)");

                if (await _repository.GetDeliveryByIdAsync(delivery.Id) == null)
                {
                    result = this.CreatedAtRoute(
                        routeName: "Default",
                        value: await _repository.AddDeliveryAsync(delivery)
                    );
                }
                else
                {
                    result = BadRequest($"The delivery with {delivery.Id} already exists");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new DeliveryDomainException("An error caused an exception", ex);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Models.Delivery>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllDeliveries(string toWhom, DateTime when)
        {
            IActionResult result = null;
            IEnumerable<Models.Delivery> deliveries = null;
            int count = 0;

            if (string.IsNullOrEmpty(toWhom)) return BadRequest($"The user {toWhom} is invalid (null or empty)");

            try
            {
                deliveries = await _repository.GetAllDeliveriesAsync(toWhom, when);
                count = deliveries?.Count() ?? 0;
                if (count == 0)
                {
                    result = NoContent();
                }
                else
                {
                    result = Ok(deliveries);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new DeliveryDomainException("An error caused an exception", ex);
            }
        }
        
        [HttpPatch("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Models.Delivery), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateDelivery(string id, [FromBody]JsonPatchDocument<Models.Delivery> patch)
        {
            IActionResult result = null;
            Models.Delivery delivery = null;

            try
            {
                if (string.IsNullOrEmpty(id)) return BadRequest($"The id {id} is not valid (null or empty)");
                if (patch == null) return BadRequest($"The patch argument is not valid");

                delivery = await _repository.GetDeliveryByIdAsync(id);
                if (delivery != null)
                {
                    patch?.ApplyTo(delivery);
                    delivery = await _repository.UpdateDeliveryAsync(id, delivery);
                    result = Ok(delivery);
                }
                else
                {
                    result = NotFound($"The delivery with id {id} was not found");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new DeliveryDomainException("An error caused an exception", ex);
            }
        }
    }
}