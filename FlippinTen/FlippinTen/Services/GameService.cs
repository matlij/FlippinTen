using Akavache;
using FlippinTen.Repository;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Models.Constants;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippinTen.Services
{
    public class GameService : BaseService, IGameService
    {
        private readonly IGenericRepository _repository;

        public GameService(IGenericRepository repository, IBlobCache cache = null) : base(cache)
        {
            _repository = repository;
        }

        public async Task<List<GamePlay>> GetGames(string playerName)
        {
            var gamesFromCache = await GetFromCache<List<GamePlay>>(playerName);
            if (gamesFromCache != null)
                return gamesFromCache;

            var uri = new UriBuilder(UriConstants.BaseUri)
            {
                Path = $"{UriConstants.GamePlayUri}{playerName}"
            };

            var games = await _repository.GetAsync<List<GamePlay>>(uri.ToString());

            await Cache.InsertObject(playerName, games, TimeSpan.FromSeconds(30));

            return games;
        }
    }
}
