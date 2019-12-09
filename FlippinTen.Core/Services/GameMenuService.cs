using Akavache;
using FlippinTen.Core.Interfaces;
using Models;
using Models.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlippinTen.Core.Services
{
    public class GameMenuService : BaseService, IGameMenuService
    {
        private readonly IGenericRepository _repository;

        public GameMenuService(IGenericRepository repository, IBlobCache cache = null) : base(cache)
        {
            _repository = repository;
        }

        public async Task<List<GamePlay>> GetGames(string playerName)
        {
            List<GamePlay> gamesFromCache = await GetFromCache<List<GamePlay>>(playerName);
            if (gamesFromCache != null)
                return gamesFromCache;

            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = UriConstants.GamePlayUri,
                Query = $"playerName={playerName}"
            };

            List<GamePlay> games = await _repository.GetAsync<List<GamePlay>>(uri.ToString());

            //await Cache.InsertObject(playerName, games, TimeSpan.FromSeconds(30));

            return games;
        }

        public async Task<GamePlay> CreateGame(string playerName, string gameName, string opponent)
        {
            if (string.IsNullOrEmpty(playerName)) throw new ArgumentNullException(nameof(playerName));
            if (string.IsNullOrEmpty(gameName)) throw new ArgumentNullException(nameof(playerName));
            if (string.IsNullOrEmpty(opponent)) throw new ArgumentNullException(nameof(opponent));

            var players = new List<Player>()
            {
                new Player(Guid.NewGuid().ToString()) { Name = playerName },
                new Player(Guid.NewGuid().ToString()) { Name = opponent }
            };
            var newGame = new GamePlay(players)
            {
                Name = gameName
            };

            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = UriConstants.GamePlayUri
            };

            GamePlay response = await _repository.PostAsync(uri.ToString(), newGame);

            //var gamesFromCache = await GetFromCache<List<GamePlay>>(playerName);
            //if (gamesFromCache != null)
            //{
            //    gamesFromCache.Add(response);
            //}

            return response;
        }
    }
}
