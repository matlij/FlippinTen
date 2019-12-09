using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Models;
using FlippinTen.Core.Utilities;
using Models;
using Models.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FlippinTen.Core.Services
{
    public class GamePlayService : IGamePlayService
    {
        private readonly IServerHubConnection _hubConnection;
        private readonly ICardGameEngine _gameEngine;

        public Player Player
        {
            get
            {
                return _gameEngine.Player;
            }
        }
        public GamePlay Game
        {
            get
            {
                return _gameEngine.Game;
            }
        }

        public bool IsAllPlayersOnline()
        {
            return _gameEngine.Game.Players.All(p => p.IsConnected);
        }
        public bool IsPlayersTurn()
        {
            return _gameEngine.Game.PlayerTurnIdentifier == Player.Identifier;
        }

        public event EventHandler<PlayerJoinedEventArgs> OnPlayerJoined;
        public event EventHandler<CardPlayedEventArgs> OnTurnedPlayed;

        public GamePlayService(IServerHubConnection hubConnection, ICardGameEngine gameEngine)
        {
            _hubConnection = hubConnection;
            _gameEngine = gameEngine;
            _hubConnection.Subscribe<PlayerJoinedEventArgs>("PlayerJoined", PlayerJoined);
            _hubConnection.Subscribe<GamePlay>("TurnedPlayed", TurnedPlayed);
        }

        public async Task<bool> ConnectToGame()
        {
            if (!await ConnectToHub())
            {
                Debug.WriteLine("Uppkoppling till server hub misslyckades.");
                return false;
            }
            Debug.WriteLine("Uppkopplad till server.");

            if (!await JoinGame())
            {
                Debug.WriteLine($"Anslutning till {_gameEngine.Game.Name} misslyckades.");
                return false;
            }
            Debug.WriteLine($"Uppkopplad till {_gameEngine.Game.Name}.");

            return true;
        }

        public async Task<bool> PlayCard(CardCollection cardCollection)
        {
            try
            {
                if (!_gameEngine.PlayCard(cardCollection.CardNr))
                    return false;

                return await _hubConnection.Invoke<bool>("PlayTurn", new object[] { _gameEngine.Game });
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Play card '{cardCollection.CardNr}' failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> PickUpCards()
        {
            if (!_gameEngine.PickUpCardsFromTable())
                return false;

            return await _hubConnection.Invoke<bool>("PlayTurn", new object[] { _gameEngine.Game });
        }

        public async Task<GamePlayResult> PlayChanceCard()
        {
            GamePlayResult result = _gameEngine.PlayChanceCard();
            if (result != GamePlayResult.InvalidPlay)
            {
                await _hubConnection.Invoke<bool>("PlayTurn", new object[] { _gameEngine.Game });

                return result;
            }

            return result;
        }

        private void PlayerJoined(PlayerJoinedEventArgs e)
        {
            _gameEngine.Game.Players.Find(p => p.Name == e.PlayerName).IsConnected = true;

            OnPlayerJoined?.Invoke(this, e);
        }

        private void TurnedPlayed(GamePlay game)
        {
            _gameEngine.Game = game;

            OnTurnedPlayed?.Invoke(this, null);
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

        private async Task<bool> JoinGame()
        {
            try
            {
                if (!await _hubConnection.Invoke<bool>("JoinGame", new object[] { _gameEngine.Game.Identifier, Player.Name }))
                    return false;

                Player.IsConnected = true;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Det gick inte att anluta till spel {_gameEngine.Game.Identifier}: {e.Message}");
                return false;
            }
        }
    }
}
