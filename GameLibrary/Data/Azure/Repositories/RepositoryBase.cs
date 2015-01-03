using GameLibrary.Data.Core;
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

        public RepositoryBase(AzureContext context, string tableName, Func<TEntity, string> rowKeyFunction, Func<TEntity, string> partitionKeyFunction)
        {
            this.context = context;
            this.TableName = tableName;
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

    }
}
