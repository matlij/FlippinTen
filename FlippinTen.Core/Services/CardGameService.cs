using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Translations;
using Models.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dto = Models.Entities;

namespace FlippinTen.Core.Services
{
    public class CardGameService : ICardGameService
    {
        private readonly IGenericRepository _repository;

        public CardGameService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CardGame>> Get(string playerName)
        {
            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = UriConstants.GamePlayUri,
                Query = $"playerName={playerName}"
            };

            var games = await _repository.GetAsync<List<dto.CardGame>>(uri.ToString());

            return games
                .Select(g => g.AsCardGame())
                .ToList();
        }

        public async Task<CardGame> Add(CardGame game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = UriConstants.GamePlayUri
            };

            var gameDto = game.AsCardGameDto();

            var response = await _repository.PostAsync(uri.ToString(), gameDto);
            if (response == null)
                return null;

            return response.AsCardGame();
        }
    }
}
