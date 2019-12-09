using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Extensions
{
    public static class PlayerExtensions
    {
        public static void AddCardsToHand(this Player player, IEnumerable<Card> cards)
        {
            var newCardAdded = false;

            foreach (var card in cards)
            {
                var cardsCollection = player.CardsOnHand.FirstOrDefault(c => c.CardNr == card.Number);

                if (cardsCollection != null)
                {
                    cardsCollection.Cards.Add(card);
                }
                else
                {
                    var newCardCollection = new CardCollection();
                    newCardCollection.Cards.Add(card);
                    player.CardsOnHand.Add(newCardCollection);

                    newCardAdded = true;
                }
            }

            if (newCardAdded)
            {
                var cardsSorted = player.CardsOnHand
                    .OrderBy(c => c.CardNr)
                    .ToList();
                player.CardsOnHand = cardsSorted;
            }
        }
    }
}
