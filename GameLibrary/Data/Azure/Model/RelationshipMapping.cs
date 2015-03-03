using GameLibrary.Data.Core;
using GameLibrary.Data.Core.Caching;
using GameLibrary.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Model
{
    public class RelationshipMapping
    {
        /// <summary>
        /// The name of the relationship, as set by the relationship attribute.
        /// </summary>
        public string Name;

        /// <summary>
        /// The id of the document that this object belongs to. Can be null if not previously assigned to a relationship.
        /// </summary>
        public string DocumentId;

        /// <summary>
        /// Indicates whether or not this object has been initialized for relationship saving. Not saved to Azure.
        /// </summary>
        [JsonIgnore]
        public bool Initialized = false;

        /// <summary>
        /// A snapshot of the relationships the last time the object was saved. Not saved to Azure.
        /// </summary>
        [JsonIgnore]
        public List<Relationship> Relationships;

        internal List<DatabaseAction> GetUpdates(RelationshipAttribute attribute, List<BaseModel> models)
        {
            //attribute.GetForeignRelationshipId(re

            return null;
        }
    }
}
