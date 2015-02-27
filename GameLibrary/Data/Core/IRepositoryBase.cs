using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Core
{
    public interface IRepositoryBase<TEntity>: ICollection<TEntity>, IRepositoryBase
        where TEntity : class, new()
    {
        List<TEntity> GetAll();
        TEntity GetById(string id);       
        void Update(TEntity entity);        
    }

    public interface IRepositoryBase
    {
        void CommitChanges();
        void RollbackChanges();
        Task CommitChangesAsync();
    }
}

