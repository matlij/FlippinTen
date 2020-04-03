using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlippinTen.Utilities
{
    public class CardGameUtilities : ICardGameUtilities
    {
        private readonly ICardUtilities _cardUtilities;

        public CardGameUtilities(ICardUtilities gameCardUtilities)
        {
            _cardUtilities = gameCardUtilities;
        }

        public CardGame CreateGame(string gameName, List<string> users)
        {
            const int cardsToHandOut = 3;

            var deckOfCards = _cardUtilities.GetDeckOfCards();
            var players = new List<Player>();
            foreach (var userIdentifier in users)
            {
                var player = new Player(userIdentifier);
                var cardsOnHand = new List<Card>();
                for (var i = 0; i < cardsToHandOut; i++)
                {
                    player.CardsHidden.Add(deckOfCards.Pop());
                    player.CardsVisible.Add(deckOfCards.Pop());
                    cardsOnHand.Add(deckOfCards.Pop());
                }

                player.AddCardsToHand(cardsOnHand);
                players.Add(player);
            }

            var id = Guid.NewGuid().ToString();

            return new CardGame(id, gameName, players, deckOfCards, new Stack<Card>(), players.First());
        }
    }
}
