﻿using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.Core.Models;
using FlippinTen.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace FlippinTenTests
{
    [TestClass]
    public class CardGameTest
    {
        private Player _player;
        private Player _opponent;
        private Card _dummyCard1;
        private Card _dummyCard2;
        private Card _dummyCardNoTen;
        private Card _dummyCardNoTwo;
        private CardGame _sut;

        [TestInitialize]
        public void Initialize()
        {
            _dummyCard1 = new Card(3, CardType.Clubs);
            _dummyCard2 = new Card(4, CardType.Dimonds);
            _dummyCardNoTen = new Card(10, CardType.Dimonds);
            _dummyCardNoTwo = new Card(2, CardType.Dimonds);
            _player = new Player("TestPlayer");
            _opponent = new Player("TestOpponent");

            var cardGameUtilities = new CardUtilities();

            var players = new List<Player> { _player, _opponent };
            var playerInformation = players
                .Select(p => new PlayerInformation(p.UserIdentifier))
                .ToList();
            playerInformation.First(p => p.Identifier == _player.UserIdentifier).IsPlayersTurn = true;
            _sut = new CardGame("TestGameId", "TestGameName", cardGameUtilities.GetDeckOfCards(), new Stack<Card>(), players.First(), playerInformation);
        }

        [TestMethod]
        public void SelectCard_NoOtherCardsSelected_ShouldSelect()
        {
            var cardHeartOne = new Card(1, CardType.Hearts);
            _player.CardsOnHand.Add(cardHeartOne);

            var result = _sut.SelectCard(cardHeartOne.ID);

            Assert.AreEqual(GamePlayResult.CardSelected, result);
            Assert.IsTrue(cardHeartOne.Selected);
        }

        [TestMethod]
        public void SelectCard_HasMoreCardsWithSameNumber_ShouldSelect()
        {
            var cardHeartOne = new Card(1, CardType.Hearts);
            var cardSpadesOne = new Card(1, CardType.Spades) { Selected = true };
            _player.CardsOnHand.Add(cardHeartOne);
            _player.CardsOnHand.Add(cardSpadesOne);

            var result = _sut.SelectCard(cardHeartOne.ID);

            Assert.AreEqual(GamePlayResult.CardSelected, result);
            Assert.IsTrue(cardHeartOne.Selected);
        }

        [TestMethod]
        public void SelectCard_TriesTpSelectedCardWithDifferentNumber_ShouldReturnInvalid()
        {
            var cardHeartOne = new Card(1, CardType.Hearts);
            var cardHeartsTwo = new Card(2, CardType.Hearts) { Selected = true };
            _player.CardsOnHand.Add(cardHeartOne);
            _player.CardsOnHand.Add(cardHeartsTwo);

            var result = _sut.SelectCard(cardHeartOne.ID);

            Assert.AreEqual(GamePlayResult.Invalid, result);
            Assert.IsFalse(cardHeartOne.Selected);
        }

        [TestMethod]
        public void SelectCard_CardSelectedTwice_ShouldUnSelect()
        {
            var cardHeartOne = new Card(1, CardType.Hearts);
            _player.CardsOnHand.Add(cardHeartOne);

            _sut.SelectCard(cardHeartOne.ID);
            Assert.IsTrue(cardHeartOne.Selected);

            _sut.SelectCard(cardHeartOne.ID);
            Assert.IsFalse(cardHeartOne.Selected);
        }

        [TestMethod]
        public void PickUpCards_CardsAddedToPlayerAndRemovedFromTabel()
        {
            //Arrange
            _sut.CardsOnTable.Push(_dummyCard1);
            _sut.CardsOnTable.Push(_dummyCard2);

            //Act
            _sut.PickUpCards();

            //Assert
            Assert.AreEqual(2, _player.CardsOnHand.Count);

            Assert.IsNotNull(_player.CardsOnHand.FirstOrDefault(c => c.Number == _dummyCard1.Number));
            Assert.IsNotNull(_player.CardsOnHand.FirstOrDefault(c => c.Number == _dummyCard2.Number));

            Assert.AreEqual(0, _sut.CardsOnTable.Count);
        }

        [TestMethod]
        public void PlayCard_TwoPlayers_ShouldUpdatePlayerTurn()
        {
            //Arrange
            _player.CardsOnHand.Add(_dummyCard1);

            //Act
            _sut.SelectCard(_dummyCard1.ID);
            _sut.PlaySelectedCards();

            //Assert
            var currentPlayer = _sut.PlayerInformation.Single(p => p.IsPlayersTurn);
            Assert.AreEqual(currentPlayer.Identifier, _opponent.UserIdentifier);
        }

        [TestMethod]
        public void PlayCard_NewCardHigher_ShouldAddCardToTable()
        {
            //Arrange
            _player.CardsOnHand.Add(_dummyCard2);
            _sut.CardsOnTable.Push(_dummyCard1);

            //Act
            _sut.SelectCard(_dummyCard2.ID);
            var result = _sut.PlaySelectedCards();

            //Assert
            Assert.AreEqual(GamePlayResult.Succeded, result);
            Assert.AreEqual(_dummyCard2.Number, _sut.CardsOnTable.Peek().Number);
        }

        [TestMethod]
        public void PlayCard_NewCardLower_ShouldNotAddCardToTable()
        {
            //Arrange
            _player.CardsOnHand.Add(_dummyCard1);
            _sut.CardsOnTable.Push(_dummyCard2);

            //Act
            _sut.SelectCard(_dummyCard1.ID);
            var result = _sut.PlaySelectedCards();

            //Assert
            Assert.AreEqual(GamePlayResult.Invalid, result);
            Assert.AreEqual(_dummyCard2.Number, _sut.CardsOnTable.Peek().Number);
        }

        [TestMethod]
        public void PlayChanceCard_ChanceCardHigher_ShouldSucceed()
        {
            //Arrange
            _sut.CardsOnTable.Push(_dummyCard1);
            _sut.DeckOfCards.Push(_dummyCard2);

            //Act
            var result = _sut.PlayChanceCard();

            //Assert
            Assert.AreEqual(GamePlayResult.Succeded, result);
        }

        [TestMethod]
        public void PlayChanceCard_ChanceCardLower_ShouldFail()
        {
            //Arrange
            _sut.CardsOnTable.Push(_dummyCard2);
            _sut.DeckOfCards.Push(_dummyCard1);

            //Act
            var result = _sut.PlayChanceCard();

            //Assert
            Assert.AreEqual(GamePlayResult.Failed, result);
        }

        [TestMethod]
        public void PlayCard_CardNrTwo_ShouldNotUpdateTurnIndex()
        {
            //Arrange
            _player.CardsOnHand.Add(_dummyCardNoTwo);

            //Act
            _sut.SelectCard(_dummyCardNoTwo.ID);
            var result = _sut.PlaySelectedCards();

            //Assert
            var currentPlayer = _sut.PlayerInformation.Single(p => p.IsPlayersTurn);
            Assert.AreEqual(GamePlayResult.Succeded, result);
            Assert.AreEqual(currentPlayer.Identifier, _player.UserIdentifier);
        }

        [TestMethod]
        public void PlayCard_CardNrTwo_ShouldAddCardToTable()
        {
            //Arrange
            _sut.CardsOnTable.Push(_dummyCard1);
            _player.CardsOnHand.Add(_dummyCardNoTwo);

            //Act
            _sut.SelectCard(_dummyCardNoTwo.ID);
            var result = _sut.PlaySelectedCards();

            //Assert
            Assert.AreEqual(GamePlayResult.Succeded, result);
            Assert.AreEqual(_dummyCardNoTwo.ID, _sut.CardsOnTable.Peek().ID);
        }

        [TestMethod]
        public void PlayCard_CardNrTen_ShouldNotUpdateTurnIndex()
        {
            //Arrange
            _player.CardsOnHand.Add(_dummyCardNoTen);

            //Act
            _sut.SelectCard(_dummyCardNoTen.ID);
            var result = _sut.PlaySelectedCards();

            //Assert
            var currentPlayer = _sut.PlayerInformation.Single(p => p.IsPlayersTurn);
            Assert.AreEqual(GamePlayResult.Succeded, result);
            Assert.AreEqual(currentPlayer.Identifier, _player.UserIdentifier);
        }

        [TestMethod]
        public void PlayCard_CardNrTen_ShouldRemovesCardsFromTable()
        {
            //Arrange
            _sut.CardsOnTable.Push(_dummyCard1);
            _player.CardsOnHand.Add(_dummyCardNoTen);

            //Act
            _sut.SelectCard(_dummyCardNoTen.ID);
            var result = _sut.PlaySelectedCards();

            //Assert
            Assert.AreEqual(GamePlayResult.Succeded, result);
            Assert.AreEqual(0, _sut.CardsOnTable.Count);
        }

        [TestMethod]
        public void UpdateGame()
        {
            var updatedCardsOnTable = new Stack<Card>();
            updatedCardsOnTable.Push(new Card(1, CardType.Hearts));
            updatedCardsOnTable.Push(new Card(2, CardType.Hearts));

            var updatedDeck = new Stack<Card>();
            updatedDeck.Push(new Card(3, CardType.Dimonds));
            updatedDeck.Push(new Card(4, CardType.Dimonds));

            _sut.UpdateGame(updatedDeck, updatedCardsOnTable, new List<PlayerInformation>());

            AssertCards(updatedCardsOnTable, _sut.CardsOnTable);
            AssertCards(updatedDeck, _sut.DeckOfCards);
        }

        private static void AssertCards(Stack<Card> expected, Stack<Card> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < actual.Count; i++)
            {
                var gameCard = actual.Pop();
                var gameDtoCard = expected.Pop();
                Assert.AreEqual(gameCard.ID, gameDtoCard.ID);
            }
        }
    }
}