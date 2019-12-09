using Models;
using System.Collections.Generic;

namespace FlippinTenWeb.DataAccess
{
    public interface IGameRepository
    {
        GamePlay Get(string identifier);
        ICollection<GamePlay> Get();
        IEnumerable<GamePlay> GetFromPlayer(string playerName);
        bool Store(GamePlay game);
        bool Update(GamePlay game);
    }
}