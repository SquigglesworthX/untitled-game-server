using GameLibrary.Data.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Core
{
    /// <summary>
    /// Class used to return a context to access game data. 
    /// </summary>
    public class ContextFactory 
    {
        /// <summary>
        /// Singleton model. 
        /// </summary>
        private ContextFactory()
        {

        }


        public static IContext GetContext()
        {
            return new AzureContext();
        }
    }
}
