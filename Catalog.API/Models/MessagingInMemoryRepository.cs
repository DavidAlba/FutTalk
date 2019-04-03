using Catalog.API.Infrastructure.DatabaseContexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Models
{
    public class MessagingInMemoryRepository : MessagingRepository, IDisposable
    {
        private bool disposed = false;

        public MessagingInMemoryRepository(SqlServerMessagingContext context) : base(context) { }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                base._context.Database.GetDbConnection().Close();
            }

            // Free any unmanaged objects here.
            
            disposed = true;
        }

        ~MessagingInMemoryRepository()
        {
            Dispose(false);
        }
    }
}
