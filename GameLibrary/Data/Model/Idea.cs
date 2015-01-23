using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Model
{
    public class Idea
    {
        public Guid IdeaId { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }

        public List<Idea> RelatedIdeas { get; set; }

    }
}
