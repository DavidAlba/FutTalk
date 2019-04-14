using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Exceptions;
using Catalog.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IMessageRepository _repository;

        public MessageController(IMessageRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [ProducesResponseType(typeof(SegmentedItems<Message>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllMessages(int size = 5, int index = 1)
        {
            IActionResult result = null;
            IEnumerable<Message> messages = null;
            int count = 0;

            if (index < 1) return BadRequest($"The index {index} must be greater than zero");

            try
            {
                messages = await _repository.GetAllMessagesAsync(size, index);
                count = messages?.Count() ?? 0;
                if (count  == 0)
                {
                    result = NoContent();
                }
                else
                {
                    result = Ok(
                        new SegmentedItems<Message>(
                            count: await _repository.LongCountAsync(),
                            index: index,
                            size: size,
                            items: messages)
                        );
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new MessagesDomainException("An error caused an exception", ex);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Message), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMessage(int id)
        {
            IActionResult result = null;
            Message message = null;

            try
            {
                if (id <= 0)
                {
                    result = BadRequest($"The id {id} is not valid");
                }
                else
                {
                    message = await _repository.GetMessageByIdAsync(id);
                    if (message != null)
                    {
                        result = Ok(message);
                    }
                    else
                    {
                        result = NotFound();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new MessagesDomainException("An error caused an exception", ex);
            }
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Message), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateMessage([FromBody]Message message)
        {
            try
            {
                IActionResult result = null;

                if (message == null) return BadRequest($"The message is invalid (null)");
                if (message.Id <= 0) return BadRequest($"The message id is invalid. Id must be greater than zero");

                if (await _repository.GetMessageByIdAsync(message.Id) == null)
                {
                    result = this.CreatedAtRoute(
                        routeName: "Default",                        
                        value: await _repository.AddMessageAsync(
                            new Message()
                            {
                                Id = message.Id,
                                Name = message?.Name,
                                Body = message?.Body
                            }
                        )
                    );
                }
                else
                {
                    result = BadRequest($"The message with {message.Id} already exists");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new MessagesDomainException("An error caused an exception", ex);
            }
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Message), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> ReplaceMessage([FromBody] Message message)
        {
            try
            {
                IActionResult result = null;

                if (message == null) return BadRequest($"The message is invalid (null)");
                if (message.Id <= 0) return BadRequest($"The message id is invalid. Id must be greater than zero");

                if (await _repository.GetMessageByIdAsync(message.Id) != null)
                {
                    result = this.CreatedAtRoute(
                        routeName: "Default",
                        value: await _repository.ReplaceMessageAsync(
                            new Message()
                            {
                                Id = message.Id,
                                Name = message?.Name,
                                Body = message?.Body
                            }
                        )
                    );
                }
                else
                {
                    result = NotFound($"The message with {message.Id} was not found");
                }

                return result;
            }

            catch (Exception ex)
            {
                throw new MessagesDomainException("An error caused an exception", ex);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            IActionResult result = null;

            try
            {
                if (id <= 0)
                {
                    result = BadRequest($"The id {id} is not valid");
                }
                else
                {
                    if (await _repository.GetMessageByIdAsync(id) != null)
                    {
                        await _repository.RemoveMessageAsync(id);
                        result = NoContent();
                    }
                    else
                    {
                        result = NotFound($"The message with {id} was not found");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new MessagesDomainException("An error caused an exception", ex);
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Message), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateMessage(int id, [FromBody]JsonPatchDocument<Message> patch)
        {
            IActionResult result = null;
            Message message = null;

            try
            {
                if (id <= 0) return BadRequest($"The id {id} is not valid. I dmust be greater than zero");
                if (patch == null) return BadRequest($"The patch argument is not valid");

                message = await _repository.GetMessageByIdAsync(id);
                if (message != null)
                {
                    patch?.ApplyTo(message);
                    message = await _repository.UpdateMessageAsync(id, message);
                    result = Ok(message);
                }
                else
                {
                    result = NotFound($"The message with id {id} was not found");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new MessagesDomainException("An error caused an exception", ex);
            }
        }
    }
}