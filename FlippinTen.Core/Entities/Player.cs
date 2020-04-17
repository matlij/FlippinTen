using Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FlippinTen.Core.Entities
{
    public class Player
    {
        public Player(string userIdentifier)
        {
            UserIdentifier = userIdentifier;
        }
        public string UserIdentifier { get; }
        public bool IsConnected { get; set; }
        public IList<CardCollection> CardsOnHand { get; private set; } = new List<CardCollection>();
        public IList<Card> CardsHidden { get; private set; } = new List<Card>();
        public IList<Card> CardsVisible { get; private set; } = new List<Card>();

        public void UpdatePlayer(IList<CardCollection> cardsOnHand, IList<Card> cardsHidden, IList<Card> cardsVisible)
        {
            CardsOnHand = cardsOnHand;
            CardsHidden = cardsHidden;
            CardsVisible = cardsVisible;
        }

        public bool PlayCardOnHand(int cardNr, out CardCollection cardCollection)
        {
            cardCollection = CardsOnHand.FirstOrDefault(c => c.CardNr == cardNr);
            if (cardCollection == null)
            {
                return false;
            }

            CardsOnHand.Remove(cardCollection);

            return true;
        }

        public void AddCardsToHand(IEnumerable<Card> cards)
        {
            var newCardAdded = false;

            foreach (var card in cards)
            {
                var cardsCollection = CardsOnHand.FirstOrDefault(c => c.CardNr == card.Number);

                if (cardsCollection != null)
                {
                    cardsCollection.Cards.Add(card);
                }
                else
                {
                    var newCardCollection = new CardCollection(card);
                    CardsOnHand.Add(newCardCollection);

                    newCardAdded = true;
                }
            }

            if (newCardAdded)
            {
                var cardsSorted = CardsOnHand
                    .OrderBy(c => c.CardNr)
                    .ToList();

                CardsOnHand = cardsSorted;
            }
        }
    }
}
