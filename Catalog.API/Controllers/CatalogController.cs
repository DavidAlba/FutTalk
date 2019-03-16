using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Infrastructure;
using Catalog.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{    
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
        public CatalogController(IRepository repository) => this.Repository = repository;

        public IRepository Repository { get; }

        [HttpGet]
        public IEnumerable<Message> Get()
        {
            try
            {
                return Repository.Messages;
            }
            catch (Exception ex)
            {
                throw new FutTalkException("An error caused an exception", ex);
            }
        }            
    }
}