using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dto = FlippinTen.Models.Entities;

namespace FlippinTen.Core.Services
{
    public class OfflineCardGameService : ICardGameOfflineService
    {
        private readonly ICardGameUtilities _gameUtilities;
        private readonly Dictionary<string, dto.CardGame> _games;

        public OfflineCardGameService(ICardGameUtilities gameUtilities)
        {
            _gameUtilities = gameUtilities;
            _games = new Dictionary<string, dto.CardGame>();
        }

        public Task<GameFlippinTen> Get(string gameIdentifier, string userIdentifier)
        {
            if (!_games.TryGetValue(gameIdentifier, out var game))
                return null;

            return Task.FromResult(game.AsCardGame(userIdentifier));
        }

        public Task<List<GameFlippinTen>> GetByPlayer(string userIdentifier)
        {
            var games = _games.Values
                .Where(g => g.Players
                    .Any(p => p.UserIdentifier == userIdentifier))
                .Select(g => g.AsCardGame(userIdentifier))
                .ToList();

            return Task.FromResult(games);
        }

        public Task<GameFlippinTen> Add(string gameName, string user, List<string> opponents)
        {
            if (gameName == null)
                throw new ArgumentNullException(nameof(gameName));

            var players = new List<string> { user };
            players.AddRange(opponents);
            var gameDto = _gameUtilities.CreateGameDto(gameName, players);

            _games.Add(gameDto.Identifier, gameDto);

            return Task.FromResult(gameDto.AsCardGame(user));
        }

        public Task<bool> Update(GameFlippinTen game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            if (!_games.TryGetValue(game.Identifier, out var gameDto))
                return Task.FromResult(false);

            var playerIndex = game.PlayerInformation.IndexOf(new PlayerInformation(game.Player.UserIdentifier));
            var playerTurnIndex = game.PlayerInformation.IndexOf(game.PlayerInformation.First(p => p.IsPlayersTurn));

            gameDto.Players[playerIndex] = game.Player.AsPlayerDto(game.PlayerInformation);
            gameDto.Players[playerTurnIndex].IsPlayersTurn = true;
            gameDto.DeckOfCards = game.DeckOfCards.AsCardStackDto();
            gameDto.CardsOnTable = game.CardsOnTable.AsCardStackDto();

            if (game.GameOver)
            {
                gameDto.GameOver = game.GameOver;
                gameDto.Winner = game.Winner;
            }

            return Task.FromResult(true);
        }
    }
}
