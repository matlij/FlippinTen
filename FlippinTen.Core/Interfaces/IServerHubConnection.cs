using System;
using System.Threading.Tasks;

namespace FlippinTen.Core.Utilities
{
    public interface IServerHubConnection
    {
        Task Disconnect();
        Task<T> Invoke<T>(string methodName, object[] parameters);
        Task SendAsync(string methodName, object[] parameters);
        Task<bool> StartConnection();
        void Subscribe<T>(string methodName, Action<T> handler);
        void Subscribe<T1, T2>(string methodName, Action<T1, T2> handler);
        void Subscribe(string methodName, Action handler);
    }
}