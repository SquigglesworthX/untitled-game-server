﻿using GameLibrary.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Repositories
{
    internal class IdeaRepository : RepositoryBase<Idea>
    {

        public IdeaRepository(AzureContext context)
            : base(context)
        {
            if (context == null)
                throw new ArgumentNullException("Context cannot be null!");
        }

    }
}
