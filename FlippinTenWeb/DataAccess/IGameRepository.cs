using Models;
using Models.Entities;
using System.Collections.Generic;

namespace FlippinTenWeb.DataAccess
{
    public interface IGameRepository
    {
        CardGame Get(string identifier);
        ICollection<CardGame> Get();
        IEnumerable<CardGame> GetFromPlayer(string userIdentifier);
        bool Store(CardGame game);
        void Update(CardGame game);
    }
}