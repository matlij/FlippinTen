using FlippinTen.Core.Entities;
using System.Collections.Generic;

namespace FlippinTen.Core.Interfaces
{
    public interface ICardGameUtilities
    {
        Entities.CardGame CreateGame(string gameName, List<string> users);
        global::Models.Entities.CardGame CreateGameDto(string gameName, List<string> users);
    }
}