using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Core
{
    /// <summary>
    /// Indicates that the targeted property is meant to be excluded from save operations to the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    internal class ExcludedAttribute : Attribute
    {
    }
}
