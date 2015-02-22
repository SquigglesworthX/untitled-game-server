using GameLibrary.Data.Azure.Identity;
using GameLibrary.Data.Azure.Model;
using GameLibrary.Data.Core;
using GameLibrary.Data.Model;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            PendingActions.Add(new AzureAction(item, ActionType.Insert));            
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
            PendingActions.Add(new AzureAction(item, ActionType.Delete));
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

        public List<TEntity> GetAll()
        {
            return dbset.GetAll();
        }

        /// <summary>
        /// Threadsafe method to commit the changes to the azure table.
        /// </summary>
        public void CommitChanges()
        {
            lock (lockobject)
            {
                foreach (AzureAction action in PendingActions.Where(t => !t.IsProcessed))
                {
                    ProcessAction(action);
                }

                PendingActions.RemoveAll(t => t.IsProcessed);
            }
        }

        protected void ProcessAction(AzureAction action)
        {
            switch (action.Action)
            {
                case ActionType.Insert:
                    dbset.Insert((TEntity)action.Model);
                    break;
                case ActionType.Delete:
                    dbset.Delete((TEntity)action.Model);
                    break;
                case ActionType.Update:
                    dbset.Update((TEntity)action.Model);
                    break;
            }

            action.IsProcessed = true;
        }


        public void Update(TEntity entity)
        {
            PendingActions.Add(new AzureAction(entity, ActionType.Update));
        }


        public void RollbackChanges()
        {
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
    }
}
