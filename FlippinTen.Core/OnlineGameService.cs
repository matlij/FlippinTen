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
    public class OnlineGameService
    {
        public event EventHandler<PlayerJoinedEventArgs> OnPlayerJoined;
        public event EventHandler<CardGameEventArgs> OnTurnedPlayed;

        private readonly ICardGameService _gameService;
        private readonly IServerHubConnection _hubConnection;
        public CardGame Game { get; private set; }

        public OnlineGameService(ICardGameService gameService, IServerHubConnection hubConnection, CardGame game)
        {
            _gameService = gameService;
            _hubConnection = hubConnection;
            Game = game;

            _hubConnection.Subscribe<string>("GameStarted", GameStarted);
            _hubConnection.SubscribeOnTurnedPlayed(TurnedPlayed);
        }

        public async Task<GameResult> Play(Func<CardGame, GameResult> play)
        {
            var result = play(Game);
            Debug.WriteLine($"OnlineGameService - Play result: {result.Result}");

            if (result.ShouldUpdateGame())
            {
                var succeded = await UpdateGame(Game);
                if (!succeded)
                {
                    Game = await _gameService.Get(Game.Identifier, Game.Player.UserIdentifier);
                    return new GameResult(Game.Identifier, Game.Player.UserIdentifier, CardPlayResult.Invalid, new Card[0]);
                }

                await BroadcastGameResult(result);
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

            if (!await JoinGame(Game.Identifier, Game.Player.UserIdentifier))
            {
                Debug.WriteLine($"Anslutning till {Game.Identifier} misslyckades.");
                return false;
            }
            Debug.WriteLine($"Uppkopplad till {Game.Identifier}.");

            return true;
        }

        public async Task Disconnect()
        {
            await _hubConnection.Disconnect();
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
                if (!await _hubConnection.Invoke<bool>("JoinGame", new object[] { gameIdentifier, userIdentifier }))
                    return false;

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Det gick inte att anluta till spel {gameIdentifier}: {e.Message}");
                return false;
            }
        }

        private async void TurnedPlayed(GameResult gameResult)
        {
            Debug.WriteLine($"Turned played by {gameResult.UserIdentifier}. Result: {gameResult.Result}");

            try
            {
                if (gameResult.ShouldUpdateGame())
                {
                    var game = await _gameService.Get(Game.Identifier, Game.Player.UserIdentifier);
                    Game.UpdateGame(game.DeckOfCards, game.CardsOnTable, game.PlayerInformation, game.GameOver, game.Winner);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Get or update game failed: " + e);
                throw;
            }

            OnTurnedPlayed?.Invoke(this, new CardGameEventArgs(gameResult));
        }

        private void GameStarted(string gameIdentifier)
        {
            Game.AllPlayersOnline = true;

            OnPlayerJoined?.Invoke(this, null);
        }

        private async Task<bool> UpdateGame(CardGame game)
        {
            try
            {
                var result = await _gameService.Update(game);
                Debug.WriteLine($"Game '{game.Identifier}' updated. Succeded: {result}");
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Update game {game.Identifier} failed: " + e);

                return false;
            }
        }

        private async Task<bool> BroadcastGameResult(GameResult gameResult)
        {
            try
            {
                var result = await _hubConnection.InvokePlayTurn(gameResult);
                Debug.WriteLine($"OnlineGameService - Game result '{gameResult.Result}' broadcasted. Succeded: {result}");
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
