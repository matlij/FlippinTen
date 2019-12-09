using System.Collections.Generic;
using Models;

namespace FlippinTen.Utilities
{
    public interface ICardGameUtilities
    {
        Stack<Card> GetDeckOfCards();
        Stack<Card> Shuffle(List<Card> cards);
    }
}