using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FlippinTenWeb.Services;
using Microsoft.AspNetCore.SignalR;
using Models;
using Models.Events;

namespace FlippinTenWeb.SignalR.Hubs
{
    public class GameHub : Hub
    {
        const string _playerNameKey = "PlayerName";
        private readonly IGameLogicLayer _gameLogic;

        public GameHub(IGameLogicLayer gameLogic)
        {
            _gameLogic = gameLogic;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.Items.TryGetValue(_playerNameKey, out var playerName))
                _gameLogic.PlayerDisconnected(playerName.ToString());

            return base.OnDisconnectedAsync(exception);
        }

        public async Task<bool> JoinGame(string gameIdentifier, string playerName)
        {
            Console.WriteLine($"{playerName} called join game. Game: {gameIdentifier}");

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
            catch (Exception e)
            {
                Console.WriteLine("Join game failed: " + e);
                return false;
            }

            Console.WriteLine("JoinGame finsihed");
            return true;
        }

        public async Task<bool> PlayTurn(GamePlay game)
        {
            Console.WriteLine($"PlayTurn called. Game {game.Name}");

            if (!_gameLogic.UpdateGame(game))
                return false;

            await Clients.GroupExcept(game.Identifier, Context.ConnectionId).SendAsync("TurnedPlayed", game);

            return true;
        }
    }
}
