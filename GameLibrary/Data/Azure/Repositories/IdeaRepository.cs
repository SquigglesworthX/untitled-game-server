using GameLibrary.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Repositories
{
    internal class IdeaRepository : RepositoryBase<Idea>
    {

        //Might be a cleaner way to do this..
        public IdeaRepository(AzureContext context)
            : base(context, "Idea", 
            //rowkey
            (t) =>
            {
                return t.Name;      
            }, 
            //partitionkey
            (t) =>
            {
                return t.IdeaId.ToString();
            })
        {
            if (context == null)
                throw new ArgumentNullException("Context cannot be null!");
        }

    }
}
