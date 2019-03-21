﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Catalog.API.Infrastructure.DatabaseContexts;

namespace Catalog.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SqliteMessagingContext>();
            services.AddSingleton<IRepository, MessagingInMemoryRepository>();
            services.AddMvc();
               //.AddXmlDataContractSerializerFormatters()
               //.AddMvcOptions(opts => {
               //    opts.FormatterMappings.SetMediaTypeMappingForFormat(
               //        "xml",
               //        new MediaTypeHeaderValue("application/xml"));
               //    opts.RespectBrowserAcceptHeader = true;
               //    opts.ReturnHttpNotAcceptable = true;
               //});
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
