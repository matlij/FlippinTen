using System;
using System.Threading.Tasks;
using FlippinTenWeb.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Models.Entities;

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
                _gameLogic.PlayerDisconnected(playerName.ToString());

            return base.OnDisconnectedAsync(exception);
        }

        public async Task<bool> JoinGame(string gameIdentifier, string userIdentifier)
        {
            if (string.IsNullOrEmpty(gameIdentifier)) return false;
            if (string.IsNullOrEmpty(userIdentifier)) return false;

            _log.LogDebug($"JoinGame called. Parameters - {nameof(gameIdentifier)}: {gameIdentifier}, {nameof(userIdentifier)}: {userIdentifier}");
            Context.Items.Add(_playerNameKey, userIdentifier);

            try
            {
                if (!_gameLogic.JoinGame(gameIdentifier, userIdentifier))
                    return false;

                await Groups.AddToGroupAsync(Context.ConnectionId, gameIdentifier);

                await Clients
                    .GroupExcept(gameIdentifier, Context.ConnectionId)
                    .SendAsync("PlayerJoined", userIdentifier);
            }
            catch (Exception e)
            {
                _log.LogError(e, $"Join game failed. {nameof(gameIdentifier)}: {gameIdentifier}, {nameof(userIdentifier)}: {userIdentifier}");
                return false;
            }

            _log.LogDebug($"JoinGame succeded.");
            return true;
        }

        public async Task<bool> PlayTurn(CardGame game)
        {
            if (!_gameLogic.UpdateGame(game))
                return false;

            await Clients.GroupExcept(game.Identifier, Context.ConnectionId).SendAsync("TurnedPlayed", game);

            return true;
        }
    }
}
