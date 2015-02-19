﻿using GameLibrary.Data.Azure.Identity;
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
        internal AzureContext context;
        internal string TableName;
        internal TableSet<TEntity> dbset;
        internal UniqueIdGenerator Generator;

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

            dbset.Insert(item);
        }
        
        // Woo. Clearing everything in one shot.
        public void Clear()
        {
            dbset.GetAll().Clear();
        }

        public bool Contains(TEntity item)
        {
            throw new NotImplementedException();
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
            get { throw new NotImplementedException(); }
        }

        public bool Remove(TEntity item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return dbset.GetAll().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public List<TEntity> GetAll()
        {
            return dbset.GetAll();
        }
    }
}
