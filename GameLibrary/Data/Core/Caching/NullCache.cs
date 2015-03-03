using GameLibrary.Data.Azure.Model;
using GameLibrary.Data.Core;
using GameLibrary.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Core.Caching
{
    internal class NullCache<TEntity> : ICache<TEntity>
    {
        protected List<DatabaseAction> PendingActions;

        public NullCache()
        {
            PendingActions = new List<DatabaseAction>();
        }

        protected void UpdateAction(DatabaseAction action)
        {
            
        }

        public void Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity GetById(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DatabaseAction> GetBatch()
        {
            throw new NotImplementedException();
        }
    }
}
