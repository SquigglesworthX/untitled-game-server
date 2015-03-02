using GameLibrary.Data.Azure.Model;
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

        public List<IRepositoryBase> Repositories;

        private RepositoryBase<Idea> ideaRepository { get; set; }
        private RepositoryBase<IdeaMapping> ideaMappingRepository { get; set; }
        private RepositoryBase<Relationship> relationshipRepository { get; set; }
        private RepositoryBase<Player> playerRepository { get; set; }

        public AzureContext(CloudStorageAccount storageAccount = null)
        {
            //Temporary for development. We should probably get rid of this constructor eventually. 
            if (storageAccount == null)
                storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            StorageAccount = storageAccount;
            Repositories = new List<IRepositoryBase>();
        }

        public AzureContext(string connectionString)
        {
            connection = connectionString;
        }

        public IRepositoryBase<Idea> IdeaRepository
        {
            get
            {
                if (ideaRepository == null)
                {
                    ideaRepository = new RepositoryBase<Idea>(this);
                    Repositories.Add(ideaRepository);
                }

                return ideaRepository;
            }
        }

        public IRepositoryBase<Relationship> RelationshipRepository
        {
            get
            {
                if (relationshipRepository == null)
                {
                    relationshipRepository = new RepositoryBase<Relationship>(this, partitionKeyFunction: (a) => a.Name + "_" + a.DocumentId);
                    Repositories.Add(relationshipRepository);
                }

                return relationshipRepository;
            }
        }

        public IRepositoryBase<IdeaMapping> IdeaMappingRepository
        {
            get
            {
                if (ideaMappingRepository == null)
                {
                    ideaMappingRepository = new RepositoryBase<IdeaMapping>(this);
                    Repositories.Add(ideaMappingRepository);
                }

                return ideaMappingRepository;
            }
        }

        public IRepositoryBase<Player> PlayerRepository
        {
            get
            {
                if (playerRepository == null)
                {
                    playerRepository = new RepositoryBase<Player>(this);
                    Repositories.Add(playerRepository);
                }

                return playerRepository;
            }
        }

        public void Commit()
        {
            List<Task> tasks = new List<Task>();
            foreach (IRepositoryBase repo in Repositories)
            {                
                tasks.Add(repo.CommitChangesAsync());
            }
            Task.WaitAll(tasks.ToArray());
        }

        public void PartialCommit(TimeSpan timeout)
        {
            List<Task> tasks = new List<Task>();
            foreach (IRepositoryBase repo in Repositories)
            {
                tasks.Add(repo.CommitPartialAsync(timeout));
            }
            Task.WaitAll(tasks.ToArray());
        }

        public void Rollback()
        {
            foreach (IRepositoryBase repo in Repositories)
            {
                repo.RollbackChanges();
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
