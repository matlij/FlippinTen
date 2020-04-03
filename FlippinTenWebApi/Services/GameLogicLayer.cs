using FlippinTenWebApi.DataAccess;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlippinTenWeb.Services
{
    public class GameLogicLayer : IGameLogicLayer
    {
        private readonly IGameRepository _gameRepository;

        public GameLogicLayer(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public IEnumerable<CardGame> GetGames(string playerName)
        {
            return playerName == null ?
                _gameRepository.Get() :
                _gameRepository.GetFromPlayer(playerName);
        }

        public bool JoinGame(string gameIdentifier, string userIdentifier)
        {
            var game = _gameRepository.Get(gameIdentifier);
            if (game == null)
            {
                return false;
            }

            var joinedPlayer = game.Players.FirstOrDefault(p => p.UserIdentifier == userIdentifier);
            if (joinedPlayer == null)
            {
                return false;
            }

            joinedPlayer.IsConnected = true;

            return _gameRepository.Update(game);
        }

        public void PlayerDisconnected(string userIdentifier)
        {
            var games = GetGames(userIdentifier);

            foreach (var game in games)
            {
                game.Players
                    .First(p => p.UserIdentifier == userIdentifier)
                    .IsConnected = false;
            }
        }

        public bool UpdateGame(CardGame game)
        {
            return _gameRepository.Update(game);
        }
    }
}
