using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace FlippinTen.Services
{
    public interface IGameService
    {
        Task<List<GamePlay>> GetGames(string playerName);
    }
}