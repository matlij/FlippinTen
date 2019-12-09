using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FlippinTenWeb.DataAccess
{
    public class GameRepository : IGameRepository
    {
        private ConcurrentDictionary<string, GamePlay> _games;

        public GameRepository()
        {
            _games = new ConcurrentDictionary<string, GamePlay>();
        }

        public bool Store(GamePlay game)
        {
            return _games.TryAdd(game.Identifier, game);
        }

        public ICollection<GamePlay> Get()
        {
            return _games.Values;
        }

        public GamePlay Get(string identifier)
        {
            if (!_games.TryGetValue(identifier, out var game))
                return null;

            return game;
        }

        public IEnumerable<GamePlay> GetFromPlayer(string playerName)
        {
            return _games.Values
                .Where(g => g.Players.FirstOrDefault(p => p.Name == playerName) != null);
        }

        public bool Update(GamePlay game)
        {
            try
            {
                _games.AddOrUpdate(game.Identifier, game, (key, oldValue) => game);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Add or update game failed: {ex.Message}");

                return false;
            }
        }
    }
}
