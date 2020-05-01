using System.Collections.Generic;
using FlippinTen.Models;
using FlippinTen.Models.Entities;

namespace FlippinTenWeb.Services
{
    public interface IGameLogicLayer
    {
        IEnumerable<CardGame> GetGames(string playerName);
        CardGame JoinGame(string gameName, string userIdentifier);
        void PlayerDisconnected(string playerName);
        bool UpdateGame(CardGame game);
    }
}