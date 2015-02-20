using GameLibrary.Data.Azure.Model;
using GameLibrary.Data.Model;
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

        /// <summary>
        /// Returns the foreign id from a relationship using the attributes known position. 
        /// </summary>
        /// <param name="relationship">The relationship to get the id from.</param>
        /// <param name="model">A model to be used in "Any" position comparisons. Can be null for first or second position.</param>
        /// <returns>The foreign key.</returns>
        public string GetForeignRelationshipId(Relationship relationship, BaseModel model = null)
        {
            //If this object is in the first position, the foreign key must be in the second.
            if (Position == RelationshipPosition.First)
            {
                return relationship.Id2;
            }
            else if (Position == RelationshipPosition.Second)
            {
                return relationship.Id1;
            }
            //If it's any, return the key that doesn't match the supplid object.
            else
            {
                return (relationship.Id1 == model.RowKey) ? relationship.Id2 : relationship.Id1;
            }
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
