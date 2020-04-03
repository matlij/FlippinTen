using FlippinTen.Utilities;
using FlippinTenWeb.DataAccess;
using FlippinTenWeb.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Models.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlippinTenTests
{
    [TestClass]
    public class GameLogicLayerTests
    {
        //private IGameLogicLayer _sut;
        //private GamePlay _game;
        //private Player _player1;
        //private Player _player2;

        //[TestInitialize]
        //public void Initialize()
        //{
        //    _player1 = new Player(Guid.NewGuid().ToString())
        //    {
        //        Name = "TestPlayer1"
        //    };

        //    _player2 = new Player(Guid.NewGuid().ToString())
        //    {
        //        Name = "TestPlayer2"
        //    };

        //    _game = new GamePlay(new List<Player>() { _player1, _player2 })
        //    {
        //        Name = "TestGame"
        //    };

        //    var repository = new Mock<IGameRepository>();
        //    repository.Setup(r => r.Get(It.IsAny<string>())).Returns(_game);
        //    repository.Setup(r => r.Store(It.IsAny<GamePlay>())).Returns(true);

        //    var deckOfCards = new CardUtilities().GetDeckOfCards();
        //    var cardService = new Mock<IGameCardUtilities>();
        //    cardService.Setup(c => c.GetDeckOfCards()).Returns(deckOfCards);

        //    _sut = new GameLogicLayer(repository.Object, cardService.Object);
        //}

        //[TestMethod]
        //public void StartGame_PlayersShouldGetCards()
        //{
        //    //Arrange
        //    //Act
        //    var game = _sut.StartGame("TestGame");

        //    //Assert

        //    foreach (var player in game.Players)
        //    {
        //        Assert.AreEqual(3, player.CardsHidden.Count);
        //        Assert.AreEqual(3, player.CardsOnHand.Count);
        //        Assert.AreEqual(3, player.CardsVisible.Count);
        //    }
        //}

        //[TestMethod]
        //public void CreateGame_PlayersShouldGetCards()
        //{
        //    //Arrange
        //    //Act
        //    var game = _sut.CreateGame(_game);

        //    //Assert
        //    foreach (var player in game.Players)
        //    {
        //        Assert.AreEqual(3, player.CardsHidden.Count);
        //        Assert.AreEqual(3, player.CardsOnHand.Count);
        //        Assert.AreEqual(3, player.CardsVisible.Count);
        //    }
        //}
    }
}
