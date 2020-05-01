using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Models.Enums;
using System;
using System.Collections.Generic;

namespace FlippinTenTests
{
    [TestClass]
    public class CardGameUtilitiesTests
    {
        private ICardGameUtilities _sut;

        [TestInitialize]
        public void Initialize()
        {
            //_dummyCard1 = new Card(2, CardType.Clubs);
            //_dummyCard2 = new Card(3, CardType.Dimonds);
            //_dummyCard3 = new Card(4, CardType.Spades);

            //_player1 = new Player(Guid.NewGuid().ToString())
            //{
            //    Name = "TestPlayer1"
            //};

            //_player2 = new Player(Guid.NewGuid().ToString())
            //{
            //    Name = "TestPlayer2"
            //};

            //_game = new GamePlay(new List<Player>() { _player1, _player2 })
            //{
            //    Name = "TestGame",
            //    CardsOnTable = new Stack<Card>()
            //};

            var cardUtils = new CardUtilities();
            _sut = new CardGameUtilities(cardUtils);
        }

        [TestMethod]
        public void CreateCardGame()
        {
            var game = _sut.CreateGameDto("TestGame", new List<string> { "p1", "p2" });

            Assert.AreEqual("TestGame", game.Name);
        }
    }
}
