using GameLibrary.Data.Azure.Repositories;
using GameLibrary.Data.Core;
using GameLibrary.Data.Model;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure
{
    internal class AzureContext : IContext
    {
        private string connection;
        public CloudStorageAccount StorageAccount { get; private set; }

        private IdeaRepository ideaRepository;

        public AzureContext()
        {
            //Temporary for development. We should probably get rid of this constructor eventually. 
            connection = "UseDevelopmentStorage=true;";
            StorageAccount = CloudStorageAccount.Parse(connection);
        }

        public AzureContext(string connectionString)
        {
            connection = connectionString;
        }

        public TableSet<TEntity> Set<TEntity>(string tableName) where TEntity : BaseModel, new()
        {
            
            var set = new TableSet<TEntity>(StorageAccount, tableName);

            return set;
        }

        public IRepositoryBase<Idea> IdeaRepository
        {
            get
            {
                if (ideaRepository == null)
                    ideaRepository = new IdeaRepository(this);

                return ideaRepository;
            }
        }

        #region IDisposable
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
        }
        #endregion
    }
}
