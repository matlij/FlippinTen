using FlippinTen.Models;
using FlippinTen.Models.Entities;
using System.Collections.Generic;

namespace FlippinTenWebApi.DataAccess
{
    public interface IGameRepository
    {
        CardGame Get(string identifier);
        ICollection<CardGame> Get();
        IEnumerable<CardGame> GetFromPlayer(string playerName);
        bool Store(CardGame game);
        bool Update(CardGame game);
    }
}