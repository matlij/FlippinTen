using Models;
using Models.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FlippinTenWeb.DataAccess
{
    public class GameRepository : IGameRepository
    {
        private ConcurrentDictionary<string, CardGame> _games;

        public GameRepository()
        {
            _games = new ConcurrentDictionary<string, CardGame>();
        }

        public bool Store(CardGame game)
        {
            return _games.TryAdd(game.Identifier, game);
        }

        public ICollection<CardGame> Get()
        {
            return _games.Values;
        }

        public CardGame Get(string identifier)
        {
            if (!_games.TryGetValue(identifier, out var game))
                return null;

            return game;
        }

        public IEnumerable<CardGame> GetFromPlayer(string userIdentifier)
        {
            return _games.Values
                .Where(g => g.Players.FirstOrDefault(p => p.UserIdentifier == userIdentifier) != null);
        }

        public void Update(CardGame game)
        {
            _games.AddOrUpdate(game.Identifier, game, (key, oldValue) => game);
        }
    }
}
