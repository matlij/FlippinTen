using FlippinTen.Core.Entities;
using FlippinTen.Core.Models;
using FlippinTen.Core.Translations;
using FlippinTen.Core.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using dto = Models.Entities;

namespace FlippinTen.Core
{
    public class OnlineGameService
    {
        public event EventHandler<PlayerJoinedEventArgs> OnPlayerJoined;
        public event EventHandler<CardPlayedEventArgs> OnTurnedPlayed;

        private readonly IServerHubConnection _hubConnection;
        
        public OnlineGameService(IServerHubConnection hubConnection)
        {
            _hubConnection = hubConnection;
            _hubConnection.Subscribe<string>("PlayerJoined", PlayerJoined);
            _hubConnection.Subscribe<dto.CardGame>("TurnedPlayed", TurnedPlayed);
        }

        public async Task<bool> Play(CardGame game, Func<CardGame, bool> play)
        {
            if (!play(game))
                return false;

            return await BroadcastGame(game);
        }

        public async Task<GamePlayResult> Play(CardGame game, Func<CardGame, GamePlayResult> play)
        {
            var result = play(game);
            if (result == GamePlayResult.InvalidPlay)
                return result;

            if (!await BroadcastGame(game))
                return GamePlayResult.InvalidPlay;

            return result;
        }

        public async Task<bool> ConnectToGame(string gameIdentifier, string userIdentifier)
        {
            if (!await ConnectToHub())
            {
                Debug.WriteLine("Uppkoppling till server hub misslyckades.");
                return false;
            }
            Debug.WriteLine("Uppkopplad till server.");

            if (!await JoinGame(gameIdentifier, userIdentifier))
            {
                Debug.WriteLine($"Anslutning till {gameIdentifier} misslyckades.");
                return false;
            }
            Debug.WriteLine($"Uppkopplad till {gameIdentifier}.");

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

        private void TurnedPlayed(dto.CardGame gameDto)
        {
            var game = gameDto.AsCardGame();

            OnTurnedPlayed?.Invoke(this, new CardPlayedEventArgs { Game = game });
        }

        private void PlayerJoined(string user)
        {
            OnPlayerJoined?.Invoke(this, new PlayerJoinedEventArgs { UserIdentifier = user });
        }

        private async Task<bool> BroadcastGame(CardGame game)
        {
            var gameDto = game.AsCardGameDto();
            return await _hubConnection.Invoke<bool>("PlayTurn", new object[] { gameDto });
        }
    }
}
