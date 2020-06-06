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

        public async void Disconnect()
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
        }

        public void SubscribeOnGameStarted<T>(string methodName, Action<T> handler)
        {
            _connection.On(methodName, handler);
        }

        public void SubscribeOnTurnedPlayed(string userIdentifier, Action<GameResult> action)
        {
            _connection.On<dtoInfo.GameResult>(
                "TurnedPlayed", 
                g => action(g.AsGameResult()));
        }

        public void SubscribeOnPlayerJoined(string userIdentifier, Action<string> action)
        {
            _connection.On("PlayerJoined", action);
        }

        public async Task<bool> JoinGame(string userIdentifier, string gameIdentifier)
        {
            return await _connection.InvokeCoreAsync<bool>("JoinGame", new object[] { gameIdentifier, userIdentifier });
        }

        public async Task<bool> InvokePlayTurn(string userIdentifier, GameResult gameResult)
        {
            var resultDto = gameResult.AsGameResultDto();
            return await _connection.InvokeCoreAsync<bool>("PlayTurn", new object[] { resultDto });
        }
    }
}
