using FlippinTen.Core;
using FlippinTen.Core.Models;
using FlippinTen.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlippinTenTests
{
    [TestClass]
    public class CardGameEngineTests
    {
        private CardGameEngine _sut;
        private GamePlay _game;
        private Player _player;
        private Player _opponent;
        private CardCollection _dummyCardCollection1;
        private CardCollection _dummyCardCollection2;
        private CardCollection _dummyCardCollectionWithCardTwo;
        private CardCollection _dummyCardCollectionWithCardTen;
        private Card _dummyCard1;
        private Card _dummyCard2;

        [TestInitialize]
        public void Initialize()
        {
            _dummyCard1 = new Card(3, CardType.Clubs);
            _dummyCard2 = new Card(4, CardType.Dimonds);

            _dummyCardCollection1 = new CardCollection { Cards = new List<Card> { _dummyCard1 } };
            _dummyCardCollection2 = new CardCollection { Cards = new List<Card> { _dummyCard2 } };
            _dummyCardCollectionWithCardTwo = new CardCollection { Cards = new List<Card> { new Card(2, CardType.Clubs) } };
            _dummyCardCollectionWithCardTen = new CardCollection { Cards = new List<Card> { new Card(10, CardType.Clubs) } };

            _player = new Player(Guid.NewGuid().ToString())
            {
                Name = "TestPlayer"
            };

            _opponent = new Player(Guid.NewGuid().ToString())
            {
                Name = "TestOpponent"
            };

            var cardGameUtilities = new CardGameUtilities();
            _game = new GamePlay(new List<Player>() { _player, _opponent })
            {
                Name = "TestGame",
                CardsOnTable = new Stack<Card>(),
                DeckOfCards = cardGameUtilities.GetDeckOfCards()
            };

            _sut = new CardGameEngine(_game, _player.Name);
        }

        [TestMethod]
        public void PickUpCards_CardsAddedToPlayerAndRemovedFromTabel()
        {
            //Arrange
            _game.CardsOnTable.Push(_dummyCard1);
            _game.CardsOnTable.Push(_dummyCard2);

            //Act
            _sut.PickUpCardsFromTable();

            //Assert
            Assert.AreEqual(2, _player.CardsOnHand.Count);

            Assert.IsNotNull(_player.CardsOnHand.FirstOrDefault(c => c.CardNr == _dummyCard1.Number));
            Assert.IsNotNull(_player.CardsOnHand.FirstOrDefault(c => c.CardNr == _dummyCard2.Number));

            Assert.AreEqual(0, _game.CardsOnTable.Count);
        }

        [TestMethod]
        public void PlayCard_TwoPlayers_ShouldUpdateTurnIndex()
        {
            //Arrange
            _player.CardsOnHand.Add(_dummyCardCollection1);

            //Act
            _sut.PlayCard(_dummyCardCollection1.CardNr);

            //Assert
            Assert.AreEqual(_opponent.Identifier, _sut.Game.PlayerTurnIdentifier);
        }

        [TestMethod]
        public void PlayCard_NewCardHigher_ShouldAddCardToTable()
        {
            //Arrange
            _player.CardsOnHand.Add(_dummyCardCollection1);

            //Act
            var result = _sut.PlayCard(_dummyCardCollection1.CardNr);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(_dummyCardCollection1.CardNr, _sut.Game.CardsOnTable.Peek().Number);
        }

        [TestMethod]
        public void PlayCard_NewCardLower_ShouldNotAddCardToTable()
        {
            //Arrange
            _player.CardsOnHand.Add(_dummyCardCollection1);
            _sut.Game.CardsOnTable.Push(_dummyCard2);

            //Act
            var result = _sut.PlayCard(_dummyCardCollection1.CardNr);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(_dummyCardCollection2.CardNr, _sut.Game.CardsOnTable.Peek().Number);
        }

        [TestMethod]
        public void PlayChanceCard_ChanceCardHigher_ShouldSucceed()
        {
            //Arrange
            _sut.Game.CardsOnTable.Push(_dummyCard1);
            _sut.Game.DeckOfCards.Push(_dummyCard2);

            //Act
            var result = _sut.PlayChanceCard();

            //Assert
            Assert.AreEqual(GamePlayResult.ChanceSucceded, result);
        }

        [TestMethod]
        public void PlayChanceCard_ChanceCardLower_ShouldFail()
        {
            //Arrange
            _sut.Game.CardsOnTable.Push(_dummyCard2);
            _sut.Game.DeckOfCards.Push(_dummyCard1);

            //Act
            var result = _sut.PlayChanceCard();

            //Assert
            Assert.AreEqual(GamePlayResult.ChanceFailed, result);
        }

        [TestMethod]
        public void PlayCard_CardNrTwo_ShouldNotUpdateTurnIndex()
        {
            //Arrange
            _player.CardsOnHand.Add(_dummyCardCollectionWithCardTwo);

            //Act
            var result = _sut.PlayCard(2);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(_player.Identifier, _game.PlayerTurnIdentifier);
        }

        [TestMethod]
        public void PlayCard_CardNrTwo_ShouldAddCardToTable()
        {
            //Arrange
            _game.CardsOnTable.Push(_dummyCard2);
            _player.CardsOnHand.Add(_dummyCardCollectionWithCardTwo);

            //Act
            var result = _sut.PlayCard(2);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(_dummyCardCollectionWithCardTwo.Cards.First().ID, _game.CardsOnTable.Peek().ID);
        }

        [TestMethod]
        public void PlayCard_CardNrTen_ShouldNotUpdateTurnIndex()
        {
            //Arrange
            _player.CardsOnHand.Add(_dummyCardCollectionWithCardTen);

            //Act
            var result = _sut.PlayCard(10);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(_player.Identifier, _game.PlayerTurnIdentifier);
        }

        [TestMethod]
        public void PlayCard_CardNrTen_ShouldRemovesCardsFromTable()
        {
            //Arrange
            _game.CardsOnTable.Push(_dummyCard2);
            _player.CardsOnHand.Add(_dummyCardCollectionWithCardTen);

            //Act
            var result = _sut.PlayCard(10);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, _game.CardsOnTable.Count);
        }
    }
}
