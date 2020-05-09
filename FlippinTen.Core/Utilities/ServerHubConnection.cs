using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Polly;
using Microsoft.AspNetCore.SignalR.Client;
using FlippinTen.Core.Models.Information;
using FlippinTen.Core.Translations;
using dtoInfo = FlippinTen.Models.Information;

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

        public void Subscribe<T>(string methodName, Action<T> handler)
        {
            _connection.On(methodName, handler);
        }

        public void SubscribeOnTurnedPlayed(Action<GameResult> action)
        {
            _connection.On<dtoInfo.GameResult>(
                "TurnedPlayed", 
                g => action(g.AsGameResult()));
        }

        public async Task<T> Invoke<T>(string methodName, object[] parameters)
        {
            return await _connection.InvokeCoreAsync<T>(methodName, parameters);
        }

        public async Task<bool> InvokePlayTurn(GameResult gameResult)
        {
            var resultDto = gameResult.AsGameResultDto();
            return await _connection.InvokeCoreAsync<bool>("PlayTurn", new object[] { resultDto });
        }
    }
}
