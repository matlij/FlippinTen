using System;
using System.Threading.Tasks;
using FlippinTen.Core.Models.Information;
using System.Collections.Generic;
using System.Linq;
using FlippinTen.Core.Interfaces;

namespace FlippinTen.Core.Utilities
{
    public class ServiceHubLocal : IServerHubConnection
    {
        Dictionary<string, Action<GameResult>> _turnedPlayed = new Dictionary<string, Action<GameResult>>();
        Dictionary<string, Action<string>> _gameStarted = new Dictionary<string, Action<string>>();
        private readonly ICardGameService _gameService;

        public ServiceHubLocal(ICardGameService gameService)
        {
            _gameService = gameService;
        }

        public Task<bool> StartConnection()
        {
            return Task.FromResult(true);
        }

        public void Disconnect()
        {
        }

        public void SubscribeOnTurnedPlayed(string userIdentifier, Action<GameResult> action)
        {
            _turnedPlayed.Add(userIdentifier, action);
        }

        public void SubscribeOnPlayerJoined(string userIdentifier, Action<string> action)
        {
            _gameStarted.Add(userIdentifier, action);
        }

        public async Task<bool> JoinGame(string gameIdentifier, string userIdentifier)
        {
            var game = await _gameService.Get(gameIdentifier, userIdentifier);
            game.Player.IsConnected = true;
            var result = await _gameService.Update(game);
            if (!result)
            {
                return result;
            }

            return await InvokeActions(userIdentifier, _gameStarted, gameIdentifier);
        }

        public Task<bool> InvokePlayTurn(string userIdentifier, GameResult gameResult)
        {
            return InvokeActions(userIdentifier, _turnedPlayed, gameResult);
        }

        private static Task<bool> InvokeActions<T>(string userIdentifier, Dictionary<string, Action<T>> keyValuePairs, T input)
        {
            var actions = keyValuePairs
                .Where(k => k.Key != userIdentifier)
                .Select(k => k.Value);

            try
            {
                foreach (var action in actions)
                {
                    action(input);
                }

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
