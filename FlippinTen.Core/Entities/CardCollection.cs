using System;
using System.Collections.Generic;
using System.Linq;

namespace FlippinTen.Core.Entities
{
    public class CardCollection
    {
        public CardCollection(Card card) : this(new List<Card> { card })
        {
        }

        public CardCollection(List<Card> cards)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards));
            }
            if (cards.Count < 1)
            {
                throw new ArgumentException("Cards list is empty");
            }
            CardNr = cards.First().Number;
            if (!cards.TrueForAll(c => c.Number == CardNr))
            {
                throw new ArgumentException("All cards in a collection must av same number. Cards in collection: " + string.Join(", ", cards.Select(c => c.CardName)));
            }

            Cards = cards;
            ImageUrl = $"spades{CardNr}.png";
        }

        public string CardNames
        {
            get
            {
                return string.Join(", ", Cards.Select(c => c.CardName));
            }
        }

        public List<Card> Cards { get; }
        public int CardNr { get; }
        public string ImageUrl { get; }
    }
}
