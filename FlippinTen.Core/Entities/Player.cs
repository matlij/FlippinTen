using FlippinTen.Models.Enums;
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
        public IList<Card> CardsOnHand { get; private set; } = new List<Card>();
        public IList<Card> CardsHidden { get; private set; } = new List<Card>();
        public IList<Card> CardsVisible { get; private set; } = new List<Card>();

        public void UpdatePlayer(IList<Card> cardsOnHand, IList<Card> cardsHidden, IList<Card> cardsVisible)
        {
            CardsOnHand = cardsOnHand;
            CardsHidden = cardsHidden;
            CardsVisible = cardsVisible;
        }

        public void AddCardsToHand(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                CardsOnHand.Add(card);
            }

            var cardsSorted = CardsOnHand
                .OrderBy(c => c.Number)
                .ToList();
            CardsOnHand = cardsSorted;
        }
    }
}
