using Akavache;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Translations;
using FlippinTen.Utilities;
using Models.Constants;
//using Models.Constants;
//using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dto = Models.Entities;

namespace FlippinTen.Core.Services
{
    public class GameMenuService : BaseService, IGameMenuService
    {
        private readonly IGenericRepository _repository;
        private readonly ICardGameUtilities _gameUtilities;

        public GameMenuService(IGenericRepository repository, ICardGameUtilities gameUtilities, IBlobCache cache = null) 
            : base(cache)
        {
            _repository = repository;
            _gameUtilities = gameUtilities;
        }

        public async Task<List<CardGame>> GetGames(string playerName)
        {
            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = UriConstants.GamePlayUri,
                Query = $"playerName={playerName}"
            };

            var games = await _repository.GetAsync<List<dto.CardGame>>(uri.ToString());

            return games.Select(g => g.AsCardGame()).ToList();
        }

        public async Task<CardGame> CreateGame(string playerIdentifier, string opponentIdentifier, string gameName)
        {
            if (string.IsNullOrEmpty(playerIdentifier)) throw new ArgumentNullException(nameof(playerIdentifier));
            if (string.IsNullOrEmpty(opponentIdentifier)) throw new ArgumentNullException(nameof(opponentIdentifier));
            if (string.IsNullOrEmpty(gameName)) throw new ArgumentNullException(nameof(gameName));

            var players = new List<string> { playerIdentifier, opponentIdentifier };
            var newGame = _gameUtilities.CreateGame(gameName, players);

            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = UriConstants.GamePlayUri
            };

            var response = await _repository.PostAsync(uri.ToString(), newGame.AsCardGameDto());
            if (response == null)
            {
                return null;
            }

            return newGame;
        }
    }
}
