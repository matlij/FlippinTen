using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlippinTen.Utilities
{
    public class CardUtilities : ICardUtilities
    {
        public Stack<Card> GetDeckOfCards()
        {
            const int cardsPerColor = 13;

            var cardsSorted = new List<Card>();

            foreach (var cardType in Enum.GetValues(typeof(CardType)).Cast<CardType>())
            {
                for (var i = 1; i < cardsPerColor + 1; i++)
                {
                    cardsSorted.Add(new Card(i, cardType));
                }
            }

            return Shuffle(cardsSorted);
        }

        public Stack<Card> Shuffle(List<Card> cards)
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
