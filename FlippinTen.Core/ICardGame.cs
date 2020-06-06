using FlippinTen.Core.Entities;
using FlippinTen.Core.Models.Information;
using System;
using System.Threading.Tasks;

namespace FlippinTen.Core
{
    public interface ICardGame
    {
        event EventHandler<PlayerJoinedEventArgs> OnPlayerJoined;
        event EventHandler<CardGameEventArgs> OnTurnedPlayed;

        Task<bool> ConnectToGame();
        void Disconnect();
        Task<GameResult> Play(Func<GameFlippinTen, GameResult> play);
        Task<GameFlippinTen> GetGame();
    }
}
