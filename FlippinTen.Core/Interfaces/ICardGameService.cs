using FlippinTen.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlippinTen.Core.Interfaces
{
    public interface ICardGameOnlineService : ICardGameService { }
    public interface ICardGameOfflineService : ICardGameService { }

    public interface ICardGameService
    {
        Task<GameFlippinTen> Get(string gameIdentifier, string userIdentifier);
        Task<List<GameFlippinTen>> GetByPlayer(string playerName);
        Task<GameFlippinTen> Add(string gameName, string user, List<string> opponents);
        Task<bool> Update(GameFlippinTen game);
    }
}