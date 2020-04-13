using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.Core.Translations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using dto = Models.Entities;
using dtoEnum = Models.Enums;

namespace FlippinTenTests
{
    [TestClass]
    public class EntityTranslationsTest
    {
        //[TestMethod]
        //public void EntityTranslation_CardGame_As_CardGameDto()
        //{
        //    var players = new List<Player>
        //    {
        //        new Player("TestPlayer1"),
        //        new Player("TestPlayer2")
        //    };
        //    var deck = CreateCardStack();
        //    var cardOnTable = CreateCardStack();
        //    var playerInfo = players.Select(p => new PlayerInformation(p.UserIdentifier)).ToList();
        //    var game = new CardGame("Id", "Name", deck, cardOnTable, players.First(), playerInfo);

        //    var gameDto = game.(playerInfo);

        //    Assert.IsNotNull(gameDto);
        //    Assert.AreEqual(game.Player.UserIdentifier, gameDto.CurrentPlayer);
        //    AssertCards(gameDto.CardsOnTable, game.CardsOnTable);
        //    AssertCards(gameDto.DeckOfCards, game.DeckOfCards);
        //}

        [TestMethod]
        public void EntityTranslation_CardGameDto_As_CardGame()
        {
            var players = new List<dto.Player>
            {
                new dto.Player(){ UserIdentifier = "TestPlayer1" },
                new dto.Player() { UserIdentifier = "TestPlayer2" }
            };

            var deck = CreateCardDtoStack();
            var cardsOnTable = CreateCardDtoStack();
            var gameDto = new dto.CardGame
            {
                CardsOnTable = cardsOnTable,
                DeckOfCards = deck,
                Identifier = "Id",
                Name = "Name",
                Players = players
            };

            var game = gameDto.AsCardGame(players.First().UserIdentifier);

            Assert.IsNotNull(game);
            Assert.AreEqual(game.Identifier, gameDto.Identifier);
            Assert.AreEqual(game.Name, gameDto.Name);
            AssertCards(gameDto.CardsOnTable, game.CardsOnTable);
            AssertCards(gameDto.DeckOfCards, game.DeckOfCards);
        }

        [TestMethod]
        public void EntityTranslation_CardStack_As_CardDtoList()
        {
            var stack = CreateCardStack();

            var stackDto = stack.AsCardStackDto();

            AssertCards(stackDto, stack);
        }

        private static Stack<Card> CreateCardStack()
        {
            var stack = new Stack<Card>();
            stack.Push(new Card(1, CardType.Hearts));
            stack.Push(new Card(2, CardType.Hearts));
            stack.Push(new Card(3, CardType.Hearts));
            return stack;
        }

        private static Stack<dto.Card> CreateCardDtoStack()
        {
            var stack = new Stack<dto.Card>();
            stack.Push(new dto.Card { ID = 1, Number = 1, CardType = dtoEnum.CardType.Hearts });
            stack.Push(new dto.Card { ID = 2, Number = 2, CardType = dtoEnum.CardType.Hearts });
            stack.Push(new dto.Card { ID = 3, Number = 3, CardType = dtoEnum.CardType.Hearts });
            return stack;
        }

        private static void AssertCards(Stack<dto.Card> cardDtoStack, Stack<Card> cardStack)
        {
            Assert.AreEqual(cardDtoStack.Count, cardStack.Count);
            for (var i = 0; i < cardStack.Count; i++)
            {
                var gameCard = cardStack.Pop();
                var gameDtoCard = cardDtoStack.Pop();
                Assert.AreEqual(gameCard.ID, gameDtoCard.ID);
            }
        }
    }
}
