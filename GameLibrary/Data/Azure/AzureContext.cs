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

        private RepositoryBase<Idea> ideaRepository;
        private RepositoryBase<IdeaMapping> ideaMappingRepository;
        private RepositoryBase<Relationship> relationshipRepository;
        private RepositoryBase<Player> playerRepository;

        public AzureContext()
        {
            //Temporary for development. We should probably get rid of this constructor eventually. 
            connection = "UseDevelopmentStorage=true;";
            StorageAccount = CloudStorageAccount.Parse(connection);
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
                return GetRepository<IRepositoryBase<Idea>>(ideaRepository, () => new RepositoryBase<Idea>(this));
                /*
                if (ideaRepository == null)
                {
                    ideaRepository = new RepositoryBase<Idea>(this);
                    Repositories.Add(ideaRepository);
                }

                return ideaRepository;
                 * */
            }
        }

        public IRepositoryBase<Relationship> RelationshipRepository
        {
            get
            {
                return GetRepository<IRepositoryBase<Relationship>>(relationshipRepository, () => new RepositoryBase<Relationship>(this, partitionKeyFunction: (a) => a.Name + "_" + a.DocumentId));
                /*
                if (relationshipRepository == null)
                {
                    relationshipRepository = new RepositoryBase<Relationship>(this, partitionKeyFunction: (a) => a.Name + "_" + a.DocumentId);
                    Repositories.Add(relationshipRepository);
                }

                return relationshipRepository;
                 * */
            }
        }

        public IRepositoryBase<IdeaMapping> IdeaMappingRepository
        {
            get
            {
                return GetRepository<IRepositoryBase<IdeaMapping>>(ideaMappingRepository, () => new RepositoryBase<IdeaMapping>(this));
                /*
                if (ideaMappingRepository == null)
                {
                    ideaMappingRepository = new RepositoryBase<IdeaMapping>(this);
                    Repositories.Add(ideaMappingRepository);
                }

                return ideaMappingRepository;
                 * */
            }
        }

        public IRepositoryBase<Player> PlayerRepository
        {
            get
            {
                return GetRepository<IRepositoryBase<Player>>(playerRepository, () => new RepositoryBase<Player>(this));
                /*
                if (playerRepository == null)
                {
                    playerRepository = new RepositoryBase<Player>(this);
                    Repositories.Add(playerRepository);
                }

                return playerRepository;
                 * */
            }
        }

        public void CommitChanges()
        {
            foreach (IRepositoryBase repo in Repositories)
            {
                repo.CommitChanges();
            }
        }

        public void Rollback()
        {
            foreach (IRepositoryBase repo in Repositories)
            {
                repo.RollbackChanges();
            }
        }

        protected t GetRepository<t>(t repo, Func<t> constructor) where t : IRepositoryBase
        {
            if (repo == null)
            {
                repo = constructor();
                Repositories.Add(repo);
            }

            return repo;
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
