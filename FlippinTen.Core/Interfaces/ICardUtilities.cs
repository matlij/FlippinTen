using FlippinTen.Core.Entities;
using System.Collections.Generic;

namespace FlippinTen.Core.Interfaces
{
    public interface ICardUtilities
    {
        Stack<Card> GetDeckOfCards();
        Stack<Card> Shuffle(List<Card> cards);
    }
}