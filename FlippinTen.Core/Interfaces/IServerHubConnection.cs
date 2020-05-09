using FlippinTen.Core.Models.Information;
using System;
using System.Threading.Tasks;

namespace FlippinTen.Core.Utilities
{
    public interface IServerHubConnection
    {
        Task<bool> StartConnection();
        Task Disconnect();
        void Subscribe<T>(string methodName, Action<T> handler);
        void SubscribeOnTurnedPlayed(Action<GameResult> action);
        Task<T> Invoke<T>(string methodName, object[] parameters);
        Task<bool> InvokePlayTurn(GameResult gameResult);
    }
}