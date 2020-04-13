using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Models;
using FlippinTen.Core.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public async Task<bool> Play(Func<CardGame, bool> play)
        {
            if (!play(Game))
                return false;

            var result = await UpdateGame(Game);
            if (!result)
            {
                Game = await _gameService.Get(Game.Identifier, Game.Player.UserIdentifier);
            }

            return result;
        }

        public async Task<GamePlayResult> Play(Func<CardGame, GamePlayResult> play)
        {
            var result = play(Game);
            if (result == GamePlayResult.InvalidPlay)
                return result;

            if (!await UpdateGame(Game))
            {
                Game = await _gameService.Get(Game.Identifier, Game.Player.UserIdentifier);
                result = GamePlayResult.InvalidPlay;
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
            try
            {
                Console.WriteLine("Game updated. Previous top card on table: " + Game.CardsOnTable.Peek().CardName);
            }
            catch (Exception)
            {

            }

            var game = await _gameService.Get(gameIdentifier, Game.Player.UserIdentifier);

            Game.UpdateGame(game.DeckOfCards, game.CardsOnTable, game.PlayerInformation);

            try
            {
                Console.WriteLine("Game updated. Top card on table: " + Game.CardsOnTable.Peek().CardName);
            }
            catch (Exception)
            {
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
            var result = await _gameService.Update(game);

            if (!result)
            {
                return false;
            }

            return await _hubConnection.Invoke<bool>("PlayTurn", new object[] { game.Identifier });
        }
    }
}
