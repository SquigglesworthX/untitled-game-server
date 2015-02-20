using GameLibrary.Data.Core;
using GameLibrary.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Model
{
    public class Relationship : BaseModel
    {
        [Excluded()]
        public string Name { get; set; }
        [Excluded()]
        public string DocumentId { get; set; }
        public string Id1 { get; set; }
        public string Id2 { get; set; }
        
    }
}
