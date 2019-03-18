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
    public class MessageController : Controller
    {
        private IRepository _repository;

        public MessageController(IRepository repository) => _repository = repository;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Message>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<Message>), (int)HttpStatusCode.NoContent)]
        public IActionResult GetMessages()
        {
            IActionResult result = null;
            IEnumerable<Message> messages = null;

            try
            {
                messages = _repository.Messages;
                if (messages?.Count() == 0)
                {
                    result = NoContent();
                }
                else
                {
                    result = Ok(messages);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new FutTalkException("An error caused an exception", ex);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Message), (int)HttpStatusCode.OK)]
        public IActionResult GetMessage(int id)
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
                    message = _repository.GetMessage(id);
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
                throw new FutTalkException("An error caused an exception", ex);                
            }
        }

        [HttpPost]        
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Message), (int)HttpStatusCode.Created)]
        public IActionResult CreateMessage([FromBody]Message message)
        {
            try
            {
                IActionResult result = null;  

                if (GetMessage(message.Id) is NotFoundResult)
                {
                    result = CreatedAtAction(
                        actionName: nameof(MessageController),
                        value: _repository.AddMessage(
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
                throw new FutTalkException("An error caused an exception", ex);
            }
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Message), (int)HttpStatusCode.Created)]
        public IActionResult ReplaceMessage([FromBody] Message message)
        {
            try
            {
                IActionResult result = null;

                if (GetMessage(message.Id) is OkObjectResult)
                {
                    result = CreatedAtAction(
                        actionName: nameof(MessageController),
                        value: _repository.ReplaceMessage(
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
                    result = BadRequest();
                }

                return result;
            }

            catch (Exception ex)
            {
                throw new FutTalkException("An error caused an exception", ex);
            }            
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult DeleteMessage(int id)
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
                    if (_repository.GetMessage(id) != null)
                    {
                        _repository.DeleteMessage(id);
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
                throw new FutTalkException("An error caused an exception", ex);
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Message), (int)HttpStatusCode.OK)]
        public IActionResult UpdateMessage(int id, [FromBody]JsonPatchDocument<Message> patch)
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
                    message = _repository.GetMessage(id);
                    if (message != null)
                    {
                        patch?.ApplyTo(message);
                        result = Ok(message);
                    }
                    else
                    {
                        result = NotFound($"The message with id {id} was not found");
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new FutTalkException("An error caused an exception", ex);
            }
        }
    }
}