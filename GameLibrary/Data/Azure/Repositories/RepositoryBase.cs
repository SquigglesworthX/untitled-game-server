using GameLibrary.Data.Azure.Identity;
using GameLibrary.Data.Azure.Model;
using GameLibrary.Data.Core;
using GameLibrary.Data.Core.Caching;
using GameLibrary.Data.Model;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Repositories
{
    internal class RepositoryBase<TEntity> : IRepositoryBase<TEntity> 
        where TEntity : BaseModel, new()
    {
        public AzureContext context;
        public string TableName;
        public TableSet<TEntity> dbset;
        public UniqueIdGenerator Generator;

        public bool IsCommittingAsync = false;

        private readonly object lockobject = new object();

        protected Func<TEntity, string> partitionKeyFunction;
        protected ICache<TEntity> Cache;

        public RepositoryBase(AzureContext context, string tableName = null, Func<TEntity, string> partitionKeyFunction = null, ICache<TEntity> cacheProvider = null)
        {
            if (context == null)
                throw new ArgumentNullException("Context cannot be null!");

            if (cacheProvider == null)
            {
                cacheProvider = new InMemoryCache<TEntity>();
            }

            this.context = context;
            this.Cache = cacheProvider;

            BlobOptimisticSyncStore store = new BlobOptimisticSyncStore(context.StorageAccount, "uniqueids", typeof(TEntity).Name + ".dat");
            Generator = new UniqueIdGenerator(store);

            if (tableName == null)
            {
                tableName = typeof(TEntity).Name;
            }
            this.TableName = tableName;

            //Should find a better key that date.
            if (partitionKeyFunction == null)
            {
                partitionKeyFunction = (t) =>
                    {
                        return DateTime.Now.Date.ToString("MMddyy");
                    };
            }

            this.partitionKeyFunction = partitionKeyFunction;
            this.dbset = new TableSet<TEntity>(context, tableName);
        }

        public TEntity GetById(string id)
        {
            return Cache.GetById(id, () => dbset.GetById(id));            
        }

        public void Add(TEntity item)
        {
            item.RowKey = Generator.GetNextId();
            item.PartitionKey = partitionKeyFunction(item);
            Cache.Add(item);
        }
        
        // Woo. Clearing everything in one shot.
        public void Clear()
        {
            dbset.GetAll().Clear();
        }

        public bool Contains(TEntity item)
        {
            if (GetById(item.RowKey) != null)
            {
                return true;
            }
            return false;
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return dbset.GetAll().Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TEntity item)
        {
            Cache.Delete(item);
            return true;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return dbset.GetAll().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return dbset.GetAll().GetEnumerator();
        }

        public void Update(TEntity entity)
        {
            Cache.Update(entity);
            //InsertAction(new DatabaseAction(entity, ActionType.Update));
        }

        /// <summary>
        /// Threadsafe method to commit the changes to the azure table.
        /// </summary>
        public void CommitChanges()
        {
            //Probably a better way to do this. 
            while (IsCommittingAsync)
            {
                Thread.Sleep(10);
            }
            lock (lockobject)
            {
                //Need to loop here
                var actions = Cache.GetBatch();

                ProcessActions(actions);                    

            }
        }

        public void RollbackChanges()
        {
            throw new NotImplementedException();
        }


        public async Task CommitChangesAsync()
        {
            while (IsCommittingAsync)
            {
                Thread.Sleep(10);
            }

            lock(lockobject)
            {
                //Another thread beat it here, might as well abandon since it's already committing.
                if (IsCommittingAsync)
                {
                    return;
                }
                IsCommittingAsync = true;
            }

            //Need to loop here
            var actions = Cache.GetBatch();

            await ProcessActionsAsync(actions);                

            IsCommittingAsync = false;
            

        }

        /// <summary>
        /// Processes actions in bulk.
        /// </summary>
        /// <param name="actions">AzureActions that all have an identical partition key.</param>
        protected void ProcessActions(IEnumerable<DatabaseAction> actions)
        {
            while (actions.Any(t => !t.IsProcessed))
            {
                var acts = actions.Where(t => !t.IsProcessed).Take(100);

                dbset.BatchOperation(acts);
            }       
        }

        /// <summary>
        /// Processes actions in bulk.
        /// </summary>
        /// <param name="actions">AzureActions that all have an identical partition key.</param>
        protected async Task ProcessActionsAsync(IEnumerable<DatabaseAction> actions)
        {
            while (actions.Any(t => !t.IsProcessed))
            {
                var acts = actions.Where(t => !t.IsProcessed).Take(100);

                await dbset.BatchOperationAsync(acts);
            }
        }

        public Task CommitPartialAsync(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }
}
