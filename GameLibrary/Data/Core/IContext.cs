using GameLibrary.Data.Azure.Model;
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
        IRepositoryBase<IdeaMapping> IdeaMappingRepository { get; }
        IRepositoryBase<Relationship> RelationshipRepository { get; }
        IRepositoryBase<Player> PlayerRepository { get; }
    }
}
