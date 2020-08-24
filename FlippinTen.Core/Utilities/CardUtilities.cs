using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace FlippinTen.Utilities
{
    public class CardUtilities : ICardUtilities
    {
        public Stack<Card> GetDeckOfCards()
        {
            const int cardsInDeck = 26;

            var cardsSorted = new List<Card>();

            for (var i = 1; i <= cardsInDeck; i++)
            {
                var card = new Card(i);
                cardsSorted.Add(card);
            }

            return Shuffle(cardsSorted);
        }

        private Stack<Card> Shuffle(List<Card> cards)
        {
            var cardsShuffled = new Stack<Card>();
            var rnd = new Random();

            var cardsCount = cards.Count;
            for (var i = 0; i < cardsCount; i++)
            {
                var rndNumber = rnd.Next(cards.Count);
                cardsShuffled.Push(cards[rndNumber]);

                cards.RemoveAt(rndNumber);
            }

            return cardsShuffled;
        }
    }
}
