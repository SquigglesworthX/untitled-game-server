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

        private RepositoryBase<Idea> ideaRepository;
        private RepositoryBase<IdeaMapping> ideaMappingRepository;
        private RepositoryBase<Relationship> relationshipRepository;
        private RepositoryBase<Player> playerRepository;

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

        public IRepositoryBase<Idea> IdeaRepository
        {
            get
            {
                if (ideaRepository == null)
                    ideaRepository = new RepositoryBase<Idea>(this);

                return ideaRepository;
            }
        }

        public IRepositoryBase<Relationship> RelationshipRepository
        {
            get
            {
                if (relationshipRepository == null)
                    relationshipRepository = new RepositoryBase<Relationship>(this, partitionKeyFunction: (a) => a.Name + "_" + a.DocumentId);

                return relationshipRepository;
            }
        }

        public IRepositoryBase<IdeaMapping> IdeaMappingRepository
        {
            get
            {
                if (ideaMappingRepository == null)
                    ideaMappingRepository = new RepositoryBase<IdeaMapping>(this);

                return ideaMappingRepository;
            }
        }

        public IRepositoryBase<Player> PlayerRepository
        {
            get
            {
                if (playerRepository == null)
                    playerRepository = new RepositoryBase<Player>(this);

                return playerRepository;
            }
        }

        public void CommitChanges()
        {
            ideaRepository.CommitChanges();
            ideaMappingRepository.CommitChanges();
            relationshipRepository.CommitChanges();
            playerRepository.CommitChanges();
        }

        public void Rollback()
        {
            ideaRepository.RollbackChanges();
            ideaMappingRepository.RollbackChanges();
            relationshipRepository.RollbackChanges();
            playerRepository.RollbackChanges();
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
