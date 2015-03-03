using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Identity
{
    public interface IOptimisticSyncStore
    {
        string GetData();
        bool TryOptimisticWrite(string data);       
    }
}
