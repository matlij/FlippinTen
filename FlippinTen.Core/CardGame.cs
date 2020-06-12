using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Models.Information;
using FlippinTen.Core.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FlippinTen.Core
{
    public class CardGame : ICardGame
    {
        public event EventHandler<PlayerJoinedEventArgs> OnPlayerJoined;
        public event EventHandler<CardGameEventArgs> OnTurnedPlayed;

        private readonly ICardGameService _gameService;
        private readonly IServerHubConnection _hubConnection;
        private readonly string _gameIdentifier;
        private readonly string _userIdentifier;

        public async Task<GameFlippinTen> GetGame() => await _gameService.Get(_gameIdentifier, _userIdentifier);

        public CardGame(ICardGameService gameService, IServerHubConnection hubConnection, string gameIdentifier, string userIdentifier)
        {
            _gameService = gameService;
            _hubConnection = hubConnection;
            _gameIdentifier = gameIdentifier;
            _userIdentifier = userIdentifier;

            _hubConnection.SubscribeOnPlayerJoined(_userIdentifier, PlayerJoined);
            _hubConnection.SubscribeOnTurnedPlayed(_userIdentifier, TurnedPlayed);
        }

        public async Task<GameResult> Play(Func<GameFlippinTen, GameResult> play)
        {
            var game = await GetGame();
            var result = play(game);
            Debug.WriteLine($"OnlineGameService - Play result: {result.Result}");

            if (result.ShouldUpdateGame())
            {
                var succeded = await UpdateGame(game);
                if (!succeded)
                {
                    game = await _gameService.Get(game.Identifier, game.Player.UserIdentifier);
                    return new GameResult(game.Identifier, game.Player.UserIdentifier, CardPlayResult.Invalid, new Card[0]);
                }

                await BroadcastGameResult(_userIdentifier, result);
            }

            return result;
        }

        public async Task<bool> ConnectToGame()
        {
            if (!await ConnectToHub())
            {
                Debug.WriteLine("Uppkoppling till server hub misslyckades.");
                return false;
            }
            Debug.WriteLine("Uppkopplad till server.");

            if (!await JoinGame(_gameIdentifier, _userIdentifier))
            {
                Debug.WriteLine($"Anslutning till {_gameIdentifier} misslyckades.");
                return false;
            }
            Debug.WriteLine($"Uppkopplad till {_gameIdentifier}.");

            return true;
        }

        public void Disconnect()
        {
            _hubConnection.Disconnect();
        }

        private async Task<bool> ConnectToHub()
        {
            try
            {
                return await _hubConnection.StartConnection();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Connect to hub failed: " + e.Message);
                return false;
            }
        }

        private async Task<bool> JoinGame(string gameIdentifier, string userIdentifier)
        {
            try
            {
                if (!await _hubConnection.JoinGame(gameIdentifier, userIdentifier))
                    return false;

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Det gick inte att anluta till spel {gameIdentifier}: {e.Message}");
                return false;
            }
        }

        private void TurnedPlayed(GameResult gameResult)
        {
            Debug.WriteLine($"Turned played by {gameResult.UserIdentifier}. Result: {gameResult.Result}");

            OnTurnedPlayed?.Invoke(this, new CardGameEventArgs(gameResult));
        }

        private void PlayerJoined(string gameIdentifier)
        {
            var eventArgs = new PlayerJoinedEventArgs { GameName = _gameIdentifier, UserIdentifier = _userIdentifier };
            OnPlayerJoined?.Invoke(this, eventArgs);
        }

        private async Task<bool> UpdateGame(GameFlippinTen game)
        {
            try
            {
                var result = await _gameService.Update(game);
                Debug.WriteLine($"game '{game.Identifier}' updated. Succeded: {result}");
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Update game {game.Identifier} failed: " + e);

                return false;
            }
        }

        private async Task<bool> BroadcastGameResult(string userIdentifier, GameResult gameResult)
        {
            try
            {
                var result = await _hubConnection.InvokePlayTurn(userIdentifier, gameResult);
                Debug.WriteLine($"OnlineGameService - game result '{gameResult.Result}' broadcasted. Succeded: {result}");
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Broadcast game result failed: " + e);

                return false;
            }
        }
    }
}
