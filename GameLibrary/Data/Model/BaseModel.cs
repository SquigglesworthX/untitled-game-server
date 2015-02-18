using GameLibrary.Data.Azure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Model
{
    /// <summary>
    /// Used to represent an Azure model base. Normally we wouldn't inherit like this as it causes a direct dependency, but it seems the easiest.
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Protected and a field so that it doesn't get serialized. Used to support relationships.
        /// </summary>
        protected List<RelationshipMapping> Relationships;

        /// <summary>
        /// Unique id used in the Azure rowkey.
        /// </summary>
        public string RowKey;

        /// <summary>
        /// Unique id used in the Azure partitionkey.
        /// </summary>
        public string PartitionKey;
    }
}
