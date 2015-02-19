using GameLibrary.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Model
{
    public class Player : BaseModel
    {
        public string Name;

        [Relationship("IdeasToPlayer", RelationshipPosition.Second)]
        public List<Idea> SubmittedIdeas { get; set; }
    }
}
