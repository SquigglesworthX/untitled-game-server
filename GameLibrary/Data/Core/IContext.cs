using GameLibrary.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Core
{
    public interface IContext : IDisposable
    {
        IRepositoryBase<Idea> IdeaRepository { get; }
    }
}
