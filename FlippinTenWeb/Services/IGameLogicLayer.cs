using System.Collections.Generic;
using Models;

namespace FlippinTenWeb.Services
{
    public interface IGameLogicLayer
    {
        IEnumerable<GamePlay> GetGames(string playerName);
        GamePlay GetGame(string identifier);
        GamePlay CreateGame(GamePlay game);
        bool JoinGame(string gameName, string playerName);
        void PlayerDisconnected(string playerName);
        void UpdateGame(GamePlay game);
    }
}