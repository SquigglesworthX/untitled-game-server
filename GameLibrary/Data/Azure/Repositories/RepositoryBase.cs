using GameLibrary.Data.Core;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Repositories
{
    internal abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> 
        where TEntity : class, new()
    {
        internal AzureContext context;
        internal string TableName;
        internal TableSet<TEntity> dbset;
        protected Func<TEntity, string> RowKeyFunction;
        protected Func<TEntity, string> partitionKeyFunction;

        public RepositoryBase(AzureContext context, string tableName, Func<TEntity, string> rowKeyFunction, Func<TEntity, string> partitionKeyFunction)
        {
            this.context = context;
            this.TableName = tableName;
            this.RowKeyFunction = rowKeyFunction;
            this.partitionKeyFunction = partitionKeyFunction;
            this.dbset = context.Set<TEntity>(tableName, rowKeyFunction, partitionKeyFunction);
        }

        public TEntity GetById(string id)
        {
            return dbset.GetById(id);
        }

        public void Add(TEntity item)
        {
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
    }
}
