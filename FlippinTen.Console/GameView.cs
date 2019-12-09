using FlippinTen.Core.Services;
using FlippinTen.Core.Utilities;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippinTen.Console
{
    public class GameView
    {
        private readonly IGameMenuService _gameService;
        private readonly IServerHubConnection _hubConnection;
        private readonly string _playerName;

        public List<GamePlay> Games { get; set; }

        public GameView(IGameMenuService gameService, IServerHubConnection hubConnection, string playerName)
        {
            _gameService = gameService;
            _hubConnection = hubConnection;
            _playerName = playerName;
        }

        public async Task<bool> Start()
        {
            var connected = await _hubConnection.StartConnection();

            if (connected)
            {
                _hubConnection.OnPlayerJoined += PlayerJoined;

                var games = await _gameService.GetGames(_playerName);

                Games.Clear();
                Games.AddRange(games);
            }

            return connected;
        }

        public async Task<bool> JoinGame(string gameName)
        {
            return await _hubConnection.JoinGame(gameName, _playerName);
        }

        private void PlayerJoined(object sender, GameReceivedEventArgs e)
        {
        }

        public void Dispose()
        {
            _hubConnection.dis
        }
    }
}
