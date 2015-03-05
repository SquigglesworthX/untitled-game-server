using GameLibrary.Data.Azure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Model
{
    public class IdeaMapping : BaseModel
    {
        public Idea Idea1 { get; set; }
        public Idea Idea2 { get; set; }
        public IdeaRelationship Relationship { get; set; }
    }

    public enum IdeaRelationship
    {
        DependsOn = 0,
        ConflictsWith
    }
}
