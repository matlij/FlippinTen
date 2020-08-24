using FlippinTen.Core.Entities;
using System.Collections.Generic;

namespace FlippinTen.Core.Interfaces
{
    public interface ICardGameUtilities
    {
        FlippinTen.Models.Entities.CardGame CreateGameDto(string gameName, List<string> users);
    }
}