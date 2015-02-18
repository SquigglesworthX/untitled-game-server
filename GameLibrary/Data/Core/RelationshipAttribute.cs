using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Core
{
    /// <summary>
    /// Represents a relationship between entities.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    internal class RelationshipAttribute : Attribute
    {
        /// <summary>
        /// The name of the relationship as it's stored in the relationship table.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The position of the id. This should be opposite for different members of the relationship. Use Any to represent a self referencing relationship.
        /// </summary>
        public RelationshipPosition Position { get; private set; }

        /// <summary>
        /// Indicates that an object has relationship with another object.
        /// </summary>
        /// <param name="name">The name of the relationship as it's stored in the relationship table.</param>
        /// <param name="position">The position of the id. These should be opposite on different members of the relationship. Use Any to represent a self referencing relationship.</param>
        public RelationshipAttribute(string name, RelationshipPosition position)
        {
            Name = name;
            Position = position;
        }
    }

    /// <summary>
    /// Where the represented object is saved in the database. 
    /// </summary>
    internal enum RelationshipPosition
    {
        First,
        Second,
        Any
    }
}
