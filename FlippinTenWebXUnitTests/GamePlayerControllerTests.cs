using FlippinTenWeb.Controllers;
using FlippinTenWeb.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FlippinTenWebXUnitTests
{
    public class GamePlayerControllerTests
    {
        private Mock<IGameLogicLayer> _gameLogicLayer;
        private GamePlay _game;
        private GamePlayController _controller;

        public GamePlayerControllerTests()
        {
            _gameLogicLayer = new Mock<IGameLogicLayer>();

            _game = new GamePlay(new List<Player>())
            {
                Identifier = "TestGame"
            };

            _gameLogicLayer.Setup(g => g.GetGame(It.IsAny<string>())).Returns(_game);

            _controller = new GamePlayController(_gameLogicLayer.Object);
        }

        [Fact]
        public void Get_ReturnGamePlay()
        {
            var result = _controller.Get("TestGame");

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GamePlay>(objectResult.Value);
            Assert.Equal("TestGame", model.Identifier);
        }

        [Fact]
        public void Patch_UpdateGame()
        {
            //var operation = new List<Operation> { new Operation("replace", nameof(GamePlay.PlayerTurnIndex), "" };
            //var patchDocument = new JsonPatchDocument();
            var test = "[{ \"op\": \"replace\", \"path\": \"PlayerTurnIndex\", \"value\": \"5\" }]";
            var json = JsonConvert.DeserializeObject<JsonPatchDocument<GamePlay>>(test);

            var result = _controller.Patch("TestGame", json);

            //var objectResult = Assert.IsType<OkObjectResult>(result);
            //var model = Assert.IsAssignableFrom<GamePlay>(objectResult.Value);
            //Assert.Equal("TestGame", model.Identifier);
        }
    }
}

