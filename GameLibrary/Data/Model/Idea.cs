using GameLibrary.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Model
{
    public class Idea : BaseModel
    {
        //public string IdeaId { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }

        [Relationship("IdeasToIdeas", RelationshipPosition.Any)]
        public List<IdeaMapping> RelatedIdeas { get; set; }

        [Relationship("IdeasToPlayer", RelationshipPosition.First)]
        public Player SubmittedBy { get; set; }

    }
}
