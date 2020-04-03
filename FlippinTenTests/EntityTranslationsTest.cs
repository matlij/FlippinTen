using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.Core.Translations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dto = Models.Entities;
using dtoEnum = Models.Enums;

namespace FlippinTenTests
{
    [TestClass]
    public class EntityTranslationsTest
    {
        [TestMethod]
        public void EntityTranslation_CardGame_As_CardGameDto()
        {
            var players = new List<Player>
            {
                new Player("TestPlayer1"),
                new Player("TestPlayer2")
            };

            var deck = new Stack<Card>();
            deck.Push(new Card(1, CardType.Clubs));
            deck.Push(new Card(2, CardType.Dimonds));

            var cardOnTable = new Stack<Card>();
            cardOnTable.Push(new Card(3, CardType.Hearts));
            cardOnTable.Push(new Card(4, CardType.Spades));

            var game = new CardGame("Id", "Name", players, deck, cardOnTable, players.First());

            var gameDto = game.AsCardGameDto();

            Assert.IsNotNull(gameDto);
            Assert.AreEqual(game.CurrentPlayer.UserIdentifier, gameDto.CurrentPlayer);
            AssertCards(gameDto.CardsOnTable, game.CardsOnTable);
            AssertCards(gameDto.DeckOfCards, game.DeckOfCards);
        }

        [TestMethod]
        public void EntityTranslation_CardGameDto_As_CardGame()
        {
            var players = new List<dto.Player>
            {
                new dto.Player(){ UserIdentifier = "TestPlayer1" },
                new dto.Player() { UserIdentifier = "TestPlayer2" }
            };

            var deck = new List<dto.Card>
            {
                new dto.Card { Number = 1, CardType = dtoEnum.CardType.Clubs },
                new dto.Card { Number = 2, CardType = dtoEnum.CardType.Dimonds }
            };

            var cardsOnTable = new List<dto.Card>
            {
                new dto.Card { Number = 3, CardType = dtoEnum.CardType.Spades },
                new dto.Card { Number = 4, CardType = dtoEnum.CardType.Hearts }
            };

            var gameDto = new dto.CardGame
            {
                CardsOnTable = cardsOnTable,
                DeckOfCards = deck,
                Identifier = "Id",
                Name = "Name",
                Players = players,
                CurrentPlayer = players.First().UserIdentifier
            };

            var game = gameDto.AsCardGame();

            Assert.IsNotNull(game);
            Assert.AreEqual(game.CurrentPlayer.UserIdentifier, gameDto.CurrentPlayer);
            Assert.AreEqual(game.Identifier, gameDto.Identifier);
            Assert.AreEqual(game.Name, gameDto.Name);
            AssertCards(gameDto.CardsOnTable, game.CardsOnTable);
            AssertCards(gameDto.DeckOfCards, game.DeckOfCards);
        }

        private static void AssertCards(List<dto.Card> cardsDto, Stack<Card> cardStack)
        {
            Assert.AreEqual(cardsDto.Count, cardStack.Count);
            var cardDtoStack = cardsDto.AsCardStack();
            for (var i = 0; i < cardStack.Count; i++)
            {
                var gameCard = cardStack.Pop();
                var gameDtoCard = cardDtoStack.Pop();
                Assert.AreEqual(gameCard.ID, gameDtoCard.ID);
            }
        }
    }
}
