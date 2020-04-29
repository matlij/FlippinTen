using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Translations;
//using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using dto = Models.Entities;

namespace FlippinTen.Utilities
{
    public class CardGameUtilities : ICardGameUtilities
    {
        private readonly ICardUtilities _cardUtilities;

        public CardGameUtilities(ICardUtilities cardUtilities)
        {
            _cardUtilities = cardUtilities;
        }

        public dto.CardGame CreateGameDto(string gameName, List<string> users)
        {
            if (users == null || users.Count == 0)
            {
                throw new ArgumentException($"{nameof(users)} missing");
            }

            const int cardsToHandOut = 3;

            var deckOfCards = _cardUtilities.GetDeckOfCards();
            var playerInfo = users
                .Select(u => new PlayerInformation(u))
                .ToList();
            playerInfo.First().IsPlayersTurn = true;
            var players = users
                .Select(u => CreatePlayer(cardsToHandOut, deckOfCards, u).AsPlayerDto(playerInfo))
                .ToList();

            return new dto.CardGame
            {
                Identifier = Guid.NewGuid().ToString(),
                Players = players,
                DeckOfCards = deckOfCards.AsCardStackDto(),
                CardsOnTable = new Stack<dto.Card>(),
                Name = gameName
            };
        }

        public CardGame CreateGame(string gameName, List<string> users)
        {
            const int cardsToHandOut = 3;

            var identifier = Guid.NewGuid().ToString();
            var deckOfCards = _cardUtilities.GetDeckOfCards();
            var cardsOnTable = new Stack<Card>();
            var players = users
                .Select(u => CreatePlayer(cardsToHandOut, deckOfCards, u))
                .ToList();
            var player = players.First();
            var playerInformation = users
                .Select(p => new PlayerInformation(p))
                .ToList();
            playerInformation.First(p => p.Identifier == player.UserIdentifier).IsPlayersTurn = true;

            return new CardGame(
                identifier,
                gameName,
                deckOfCards,
                cardsOnTable,
                player,
                playerInformation);
        }

        private static Player CreatePlayer(int cardsToHandOut, Stack<Card> deckOfCards, string userIdentifier)
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

            return player;
        }
    }
}
