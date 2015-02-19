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

        /// <summary>
        /// Class extending the base repository and overrides it with Idea specific functions. Not used anymore. Might be worth scrapping.
        /// </summary>        
        public IdeaRepository(AzureContext context)
            : base(context)
        {

        }

    }
}
