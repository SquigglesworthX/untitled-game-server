using GameLibrary.Data.Azure.Model;
using GameLibrary.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Model
{
    /// <summary>
    /// Used to represent an Azure model base. Normally we wouldn't inherit like this as it causes a direct dependency, but it seems the easiest.
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Used to support relationships.
        /// </summary>
        public List<RelationshipMapping> Relationships { get; set; }

        /// <summary>
        /// Unique id used in the Azure rowkey.
        /// </summary>
        [Excluded]
        public string RowKey { get; set; }

        /// <summary>
        /// Unique id used in the Azure partitionkey.
        /// </summary>
        [Excluded]
        public string PartitionKey { get; set; }

        /// <summary>
        /// Identifier used in optimistic concurrency for Azure Storage.
        /// </summary>
        [Excluded]
        public string ETag { get; set; }
    }
}
