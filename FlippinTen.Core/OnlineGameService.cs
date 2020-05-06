using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FlippinTen.Core
{
    public class OnlineGameService
    {
        public event EventHandler<PlayerJoinedEventArgs> OnPlayerJoined;
        public event EventHandler<CardPlayedEventArgs> OnTurnedPlayed;

        private readonly ICardGameService _gameService;
        private readonly IServerHubConnection _hubConnection;
        public CardGame Game { get; private set; }

        public OnlineGameService(ICardGameService gameService, IServerHubConnection hubConnection, CardGame game)
        {
            _gameService = gameService;
            _hubConnection = hubConnection;
            Game = game;

            _hubConnection.Subscribe<string>("GameStarted", GameStarted);
            _hubConnection.Subscribe<string>("TurnedPlayed", TurnedPlayed);
        }

        public async Task<GamePlayResult> Play(Func<CardGame, GamePlayResult> play)
        {
            var result = play(Game);
            Debug.WriteLine($"Play result: {result}");

            if (result != GamePlayResult.Succeded)
                return result;

            if (!await UpdateGame(Game))
            {
                Debug.WriteLine($"Update game failed.");

                Game = await _gameService.Get(Game.Identifier, Game.Player.UserIdentifier);
                result = GamePlayResult.Invalid;
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

        private async void TurnedPlayed(string gameIdentifier)
        {
            Debug.WriteLine($"Turned played {gameIdentifier}");

            try
            {
                var game = await _gameService.Get(gameIdentifier, Game.Player.UserIdentifier);
                Game.UpdateGame(game.DeckOfCards, game.CardsOnTable, game.PlayerInformation, game.GameOver, game.Winner);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Get or update game failed: " + e);
                throw;
            }


            OnTurnedPlayed?.Invoke(this, null);
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

                if (!result)
                {
                    return false;
                }

                return await _hubConnection.Invoke<bool>("PlayTurn", new object[] { game.Identifier });
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Update game {game.Identifier} failed: " + e);

                return false;
            }
        }
    }
}
