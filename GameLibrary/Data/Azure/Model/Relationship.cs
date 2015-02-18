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
        public string Name;
        [Excluded()]
        public string DocumentId;
        public string Id1;
        public string Id2;        
        
    }
}
