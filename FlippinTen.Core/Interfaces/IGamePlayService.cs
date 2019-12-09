using FlippinTen.Core.Models;
using Models;
using Models.Events;
using System;
using System.Threading.Tasks;

namespace FlippinTen.Core.Interfaces
{
    public interface IGamePlayService
    {
        Player Player { get; }
        GamePlay Game { get; }
        bool IsAllPlayersOnline();
        bool IsPlayersTurn();
        Task<bool> ConnectToGame();
        Task<bool> PlayCard(CardCollection cardCollection);
        Task<bool> PickUpCards();
        Task<GamePlayResult> PlayChanceCard();

        event EventHandler<PlayerJoinedEventArgs> OnPlayerJoined;

        event EventHandler<CardPlayedEventArgs> OnTurnedPlayed;
    }
}