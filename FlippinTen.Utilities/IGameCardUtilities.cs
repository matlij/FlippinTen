using System.Collections.Generic;
using Models;
using Models.Entities;

namespace FlippinTen.Utilities
{
    public interface IGameCardUtilities
    {
        Stack<Card> GetDeckOfCards();
        Stack<Card> Shuffle(List<Card> cards);
    }
}