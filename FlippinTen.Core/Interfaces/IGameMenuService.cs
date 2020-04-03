using FlippinTen.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlippinTen.Core.Interfaces
{
    public interface IGameMenuService
    {
        Task<List<CardGame>> GetGames(string playerName);
        Task<CardGame> CreateGame(string playerName, string gameName, string opponent);
    }
}