using GameLibrary.Data.Azure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Repositories
{
    internal class RelationshipRepository : RepositoryBase<Relationship>
    {
        public RelationshipRepository(AzureContext context)
            : base(context, partitionKeyFunction: (a) => a.Name + "_" + a.DocumentId)
        {

        }
    }
}
