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
    internal class InMemoryCache<TEntity> : ICache<TEntity> where TEntity : BaseModel
    {
        protected List<DatabaseAction> PendingActions;
        public List<TEntity> Entities;

        public InMemoryCache()
        {
            Entities = new List<TEntity>();
            PendingActions = new List<DatabaseAction>();
        }

        protected void UpdateAction(DatabaseAction action)
        {
            TEntity model = (TEntity)action.Model;
            bool match = false;
            foreach (DatabaseAction act in PendingActions)
            {
                TEntity entity = (TEntity)act.Model;
                if (entity.RowKey == model.RowKey)
                {
                    act.UpdateAction(action.Action);
                    match = true;
                    break;
                }
            }

            if (!match)
            {
                PendingActions.Add(action);
            }
        }

        public void Add(TEntity entity)
        {
            Entities.Add(entity);
            PendingActions.Add(new DatabaseAction(entity, ActionType.Insert));
        }

        public void Update(TEntity entity)
        {
            //Attach if not attached.
            if (Entities.FirstOrDefault(t => t.RowKey == entity.RowKey) == null)
            {
                Entities.Add(entity);
            }
            UpdateAction(new DatabaseAction(entity, ActionType.Update));

        }

        public void Delete(TEntity entity)
        {
            //Attach if not attached.
            if (Entities.FirstOrDefault(t => t.RowKey == entity.RowKey) == null)
            {
                Entities.Add(entity);
            }
            UpdateAction(new DatabaseAction(entity, ActionType.Delete));
        }

        public TEntity GetById(string id, Func<TEntity> databaseCall)
        {
            TEntity model = Entities.FirstOrDefault(t => t.RowKey == id);
            if (model == null)
            {
                model = databaseCall();
            }
            return model;
        }

        public IEnumerable<DatabaseAction> GetBatch()
        {            
            var key = ((BaseModel)PendingActions.Where(t=>!t.IsProcessed).OrderBy(t => t.Timestamp).First().Model).RowKey;

            var actions = PendingActions.Where(t => ((BaseModel)t.Model).RowKey == key).Take(100);
            foreach (DatabaseAction action in actions)
            {
                action.IsProcessed = true;
            }
            return actions;
        }


        public IEnumerable<DatabaseAction> GetBatch(TimeSpan timespan)
        {           
            var key = ((BaseModel)PendingActions.Where(t => !t.IsProcessed && DateTime.Now.Subtract(t.Timestamp) > timespan).OrderBy(t => t.Timestamp).First().Model).RowKey;

            var actions = PendingActions.Where(t => ((BaseModel)t.Model).RowKey == key).Take(100);
            foreach (DatabaseAction action in actions)
            {
                action.IsProcessed = true;
            }
            return actions;                
        }
    }
}
