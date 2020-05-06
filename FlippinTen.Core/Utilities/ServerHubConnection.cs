using Microsoft.AspNetCore.SignalR.Client;
using Polly;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FlippinTen.Core.Utilities
{
    public class ServerHubConnection : IServerHubConnection
    {
        private readonly HubConnection _connection;

        public ServerHubConnection(IHubConnectionBuilder hubConnectionBuilder, string url)
        {
            _connection = hubConnectionBuilder
                .WithUrl(url)
                .Build();
        }

        public void Subscribe(string methodName, Action handler)
        {
            _connection.On(methodName, handler);
        }

        public void Subscribe<T>(string methodName, Action<T> handler)
        {
            _connection.On(methodName, handler);
        }

        public void Subscribe<T1, T2>(string methodName, Action<T1, T2> handler)
        {
            _connection.On(methodName, handler);
        }

        public async Task<T> Invoke<T>(string methodName, object[] parameters)
        {
            return await _connection.InvokeCoreAsync<T>(methodName, parameters);
        }

        public async Task SendAsync(string methodName, object[] parameters)
        {
            await _connection.SendAsync(methodName, parameters);
        }

        public async Task<bool> StartConnection()
        {
            try
            {
                await Policy.Handle<Exception>(e =>
                {
                    Debug.WriteLine("Connection to hub failed. " + e.Message);
                    return true;
                })
                .WaitAndRetryAsync(5, retryAttempt =>
                {
                    Console.WriteLine($"Failed to connect to Hub. Trying again - Attempt '{retryAttempt}'");
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                })
                .ExecuteAsync(async () => await _connection.StartAsync());

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Couldnt open connection: " + e);
                return false;
            }
        }

        public async Task Disconnect()
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
        }
    }
}
