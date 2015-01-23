using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Core
{
    public interface IRepositoryBase<TEntity> where TEntity : class, new()
    {
        //Need to add in a caching layer here plus any relevant functions (search by Id, etc)
        List<TEntity> GetAll();
        void Insert(TEntity entity);
        TEntity GetById(string id);
    }
}

