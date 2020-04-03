using Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlippinTen.Core.Utilities
{
    public class GamePlayUtilities
    {
        private readonly IGameCardUtilities _gameCardUtilities;

        public GamePlayUtilities(IGameCardUtilities gameCardUtilities)
        {
            _gameCardUtilities = gameCardUtilities;
        }

        public CardGame CreateGame()
        {
            var game = new CardGame();

            game.DeckOfCards = _gameCardUtilities.GetDeckOfCards();

            const int cardsToHandOut = 3;
            foreach (var player in game.Players)
            {
                var cardsOnHand = new List<Card>();
                for (var i = 0; i < cardsToHandOut; i++)
                {
                    player.CardsHidden.Add(game.DeckOfCards.Pop());
                    player.CardsVisible.Add(game.DeckOfCards.Pop());
                    cardsOnHand.Add(game.DeckOfCards.Pop());
                }
                player.AddCardsToHand(cardsOnHand);
            }

            game.CardsOnTable = new Stack<Card>();

            game.Identifier = Guid.NewGuid().ToString();
        }
    }
}
