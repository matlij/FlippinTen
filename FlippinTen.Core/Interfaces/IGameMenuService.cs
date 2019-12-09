using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlippinTen.Core.Interfaces
{
    public interface IGameMenuService
    {
        Task<List<GamePlay>> GetGames(string playerName);
        Task<GamePlay> CreateGame(string playerName, string gameName, string opponent);
    }
}