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

        //Right now lists are ignored. I'm going to work on automatically converting these to something Azure can work with behind the scenes in the TableEntityDTO object.
        //It'll probably have to be simplified to a list of strings containing the ids. 
        public List<Idea> RelatedIdeas;

    }
}
