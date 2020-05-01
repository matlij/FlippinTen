using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Translations;
using Microsoft.AspNetCore.JsonPatch;
using FlippinTen.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dto = FlippinTen.Models.Entities;

namespace FlippinTen.Core.Services
{
    public class CardGameService : ICardGameService
    {
        private readonly IGenericRepository _repository;
        private readonly ICardGameUtilities _gameUtilities;

        public CardGameService(IGenericRepository repository, ICardGameUtilities gameUtilities)
        {
            _repository = repository;
            _gameUtilities = gameUtilities;
        }

        public async Task<CardGame> Get(string gameIdentifier, string userIdentifier)
        {
            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = $"{UriConstants.GamePlayUri}/{gameIdentifier}"
            };

            var game = await _repository.GetAsync<dto.CardGame>(uri.ToString());
            return game.AsCardGame(userIdentifier);
        }

        public async Task<List<CardGame>> GetByPlayer(string userIdentifier)
        {
            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = UriConstants.GamePlayUri,
                Query = $"playerName={userIdentifier}"
            };

            var games = await _repository.GetAsync<List<dto.CardGame>>(uri.ToString());

            return games
                .Select(g => g.AsCardGame(userIdentifier))
                .ToList();
        }

        public async Task<CardGame> Add(string gameName, string user, List<string> opponents)
        {
            if (gameName == null)
                throw new ArgumentNullException(nameof(gameName));

            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = UriConstants.GamePlayUri
            };

            var players = new List<string> { user };
            players.AddRange(opponents);
            var gameDto = _gameUtilities.CreateGameDto(gameName, players);

            var response = await _repository.PostAsync(uri.ToString(), gameDto);
            if (response == null)
                return null;

            return response.AsCardGame(user);
        }

        public async Task<bool> Update(CardGame game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = $"{UriConstants.GamePlayUri}/{game.Identifier}"
            };

            var playerIndex = game.PlayerInformation.IndexOf(new PlayerInformation(game.Player.UserIdentifier));
            var playerTurnIndex = game.PlayerInformation.IndexOf(game.PlayerInformation.First(p => p.IsPlayersTurn));

            var patch = new JsonPatchDocument<dto.CardGame>();
            patch.Replace(g => g.Players[playerIndex], game.Player.AsPlayerDto(game.PlayerInformation));
            patch.Replace(g => g.Players[playerTurnIndex].IsPlayersTurn, true);
            patch.Replace(g => g.DeckOfCards, game.DeckOfCards.AsCardStackDto());
            patch.Replace(g => g.CardsOnTable, game.CardsOnTable.AsCardStackDto());

            if (game.GameOver)
            {
                patch.Replace(g => g.GameOver, game.GameOver);
                patch.Replace(g => g.Winner, game.Winner);
            }

            var result = await _repository.PatchAsync(uri.ToString(), patch);
            return result;
        }
    }
}
