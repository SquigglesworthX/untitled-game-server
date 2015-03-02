using GameLibrary.Data.Azure.Identity;
using GameLibrary.Data.Azure.Model;
using GameLibrary.Data.Core;
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

        protected List<AzureAction> PendingActions = new List<AzureAction>();

        protected Func<TEntity, string> partitionKeyFunction;

        public RepositoryBase(AzureContext context, string tableName = null, Func<TEntity, string> partitionKeyFunction = null)
        {
            if (context == null)
                throw new ArgumentNullException("Context cannot be null!");

            this.context = context;

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
            return dbset.GetById(id);
        }

        public void Add(TEntity item)
        {
            item.RowKey = Generator.GetNextId();
            item.PartitionKey = partitionKeyFunction(item);
            InsertAction(new AzureAction(item, ActionType.Insert));     
        }
        
        // Woo. Clearing everything in one shot.
        public void Clear()
        {
            dbset.GetAll().Clear();
        }

        public bool Contains(TEntity item)
        {
            if (dbset.GetById(item.RowKey) != null)
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
            InsertAction(new AzureAction(item, ActionType.Delete));
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
            InsertAction(new AzureAction(entity, ActionType.Update));
        }

        protected void InsertAction(AzureAction action)
        {
            PendingActions.Add(action);
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

                var actions = PendingActions.Where(t => !t.IsProcessed).GroupBy(t => t.Model.PartitionKey);
                
                foreach(IGrouping<string, AzureAction> group in actions)
                {
                    ProcessActions(group);                    
                }

                PendingActions.RemoveAll(t => t.IsProcessed);
            }
        }

        public void RollbackChanges()
        {
            while (IsCommittingAsync)
            {
                Thread.Sleep(10);
            }
            //Use the same lockobject - we want to also prevent rollback and committing at the same time. 
            lock (lockobject)
            {
                foreach (AzureAction action in PendingActions.Where(t => !t.IsProcessed))
                {
                    action.UpdateAction(ActionType.Ignore);
                    action.IsProcessed = true;
                }

                PendingActions.RemoveAll(t => t.IsProcessed);
            }
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

            var actions = PendingActions.Where(t => !t.IsProcessed).GroupBy(t => t.Model.PartitionKey);

            foreach (IGrouping<string, AzureAction> group in actions)
            {
                await ProcessActionsAsync(group);
            }

            PendingActions.RemoveAll(t => t.IsProcessed);

            IsCommittingAsync = false;
            

        }

        /// <summary>
        /// Processes actions in bulk.
        /// </summary>
        /// <param name="actions">AzureActions that all have an identical partition key.</param>
        protected void ProcessActions(IEnumerable<AzureAction> actions)
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
        protected async Task ProcessActionsAsync(IEnumerable<AzureAction> actions)
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
