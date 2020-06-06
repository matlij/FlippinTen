using FlippinTen.Core.Models.Information;
using System;
using System.Threading.Tasks;

namespace FlippinTen.Core.Utilities
{
    public interface IServerHubConnection
    {
        Task<bool> StartConnection();
        void Disconnect();
        void SubscribeOnPlayerJoined(string userIdentifier, Action<string> action);
        void SubscribeOnTurnedPlayed(string userIdentifier, Action<GameResult> action);
        Task<bool> InvokePlayTurn(string userIdentifier, GameResult gameResult);
        Task<bool> JoinGame(string userIdentifier, string gameIdentifier);
    }
}