using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FlippinTenWeb.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Models;
using Models.Events;
using Newtonsoft.Json;

namespace FlippinTenWeb.SignalR.Hubs
{
    public class GameHub : Hub
    {
        const string _playerNameKey = "PlayerName";
        private readonly IGameLogicLayer _gameLogic;
        private readonly ILogger<GameHub> _log;

        public GameHub(IGameLogicLayer gameLogic, ILogger<GameHub> log)
        {
            _gameLogic = gameLogic;
            _log = log;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _log.LogInformation($"Player disconnected: {exception.Message}");

            try
            {
                if (Context.Items.TryGetValue(_playerNameKey, out var playerName))
                    _gameLogic.PlayerDisconnected(playerName.ToString());
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Disconnect player failed.", exception);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task<bool> JoinGame(string gameIdentifier, string playerName)
        {
            if (string.IsNullOrEmpty(gameIdentifier)) return false;
            if (string.IsNullOrEmpty(playerName)) return false;

            Context.Items.Add(_playerNameKey, playerName);

            try
            {
                if (!_gameLogic.JoinGame(gameIdentifier, playerName))
                    return false;

                await Groups.AddToGroupAsync(Context.ConnectionId, gameIdentifier);

                await Clients.GroupExcept(gameIdentifier, Context.ConnectionId)
                    .SendAsync("PlayerJoined", new PlayerJoinedEventArgs() { GameName = gameIdentifier, PlayerName = playerName });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Join game failed", gameIdentifier, playerName);
                return false;
            }


            return true;
        }

        public async Task<bool> PlayTurn(GamePlay game)
        {
            try
            {
                _gameLogic.UpdateGame(game);

                await Clients.GroupExcept(game.Identifier, Context.ConnectionId).SendAsync("TurnedPlayed", game);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Update game failed: " + JsonConvert.SerializeObject(game));

                return false;
            }

            return true;
        }
    }
}
