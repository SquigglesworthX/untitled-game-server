using GameLibrary.Data.Core;
using GameLibrary.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Core.Caching
{
    public interface ICache
    {
        IEnumerable<DatabaseAction> GetBatch();
        IEnumerable<DatabaseAction> GetBatch(TimeSpan timespan);
    }

    public interface ICache<TEntity> : ICache
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        TEntity GetById(string id,Func<TEntity> databaseCall);
    }
}
