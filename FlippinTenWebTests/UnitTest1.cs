using FlippinTenWeb.Controllers;
using FlippinTenWeb.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;

namespace FlippinTenWebTests
{
    [TestClass]
    public class UnitTest1
    {
        private Mock<IGameLogicLayer> _gameLogicLayer;
        private GamePlay _game;
        private GamePlayController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _gameLogicLayer = new Mock<IGameLogicLayer>();

            _game = new GamePlay
            {
                Identifier = "TestGame"
            };

            _gameLogicLayer.Setup(g => g.GetGame(It.IsAny<string>())).Returns(_game);

            _controller = new GamePlayController(_gameLogicLayer.Object);
        }

        [TestMethod]
        public void Get_ReturnGame()
        {
            var result = _controller.Get("TestGame");

            Assert.IsInstanceOfType<IEnumerable<GamePlay>>()
        }
    }
}
