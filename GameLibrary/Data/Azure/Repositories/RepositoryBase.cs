using GameLibrary.Data.Core;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Repositories
{
    internal abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class, new()
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

        public virtual List<TEntity> GetAll()
        {
            return dbset.GetAll();
        }

        public virtual void Insert(TEntity entity)
        {
            dbset.Insert(entity);
        }

        public TEntity GetById(string id)
        {
            return dbset.GetById(id);
        }
    }
}
