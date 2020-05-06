using System;
using System.Threading.Tasks;
using FlippinTenWeb.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using FlippinTen.Models.Entities;

namespace FlippinTenWebApi.SignalR.Hubs
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
            if (Context.Items.TryGetValue(_playerNameKey, out var playerName))
            { 
                _gameLogic.PlayerDisconnected(playerName.ToString());
                Context.Items.Remove(_playerNameKey);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task<bool> JoinGame(string gameIdentifier, string userIdentifier)
        {
            if (string.IsNullOrEmpty(gameIdentifier)) return false;
            if (string.IsNullOrEmpty(userIdentifier)) return false;

            _log.LogDebug($"JoinGame called. Parameters - {nameof(gameIdentifier)}: {gameIdentifier}, {nameof(userIdentifier)}: {userIdentifier}");
    
            if(!Context.Items.ContainsKey(_playerNameKey))
                Context.Items.Add(_playerNameKey, userIdentifier);

            try
            {
                var game = _gameLogic.JoinGame(gameIdentifier, userIdentifier);
                if (game == null)
                {
                    return false;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, gameIdentifier);

                if (game.Players.TrueForAll(p => p.IsConnected))
                {
                    await Clients
                        .Group(gameIdentifier)
                        .SendAsync("GameStarted", userIdentifier);
                }
            }
            catch (Exception e)
            {
                _log.LogError(e, $"Join game failed. {nameof(gameIdentifier)}: {gameIdentifier}, {nameof(userIdentifier)}: {userIdentifier}");
                return false;
            }

            _log.LogDebug($"JoinGame succeded.");
            return true;
        }

        public async Task<bool> PlayTurn(string gameIdentifier)
        {
            await Clients.GroupExcept(gameIdentifier, Context.ConnectionId).SendAsync("TurnedPlayed", gameIdentifier);

            return true;
        }
    }
}
