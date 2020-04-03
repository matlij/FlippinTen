using FlippinTenWebApi.Controllers;
using FlippinTenWebApi.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace FlippinTenWebXUnitTests
{
    public class CardGameerControllerTests
    {
        private Mock<IGameRepository> _repositoy;
        private CardGame _game;
        private GameController _controller;

        public CardGameerControllerTests()
        {
            _repositoy = new Mock<IGameRepository>();

            _game = new CardGame()
            {
                Identifier = "TestId",
                Name = "TestName",
                Players = new List<Player>()
                { 
                    new Player()
                }
            };

            _repositoy.Setup(g => g.Store(It.IsAny<CardGame>())).Returns(true);

            _controller = new GameController(_repositoy.Object, new Mock<ILogger<GameController>>().Object);
        }

        [Fact]
        public void Post()
        {
            var result = _controller.Post(_game);

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CardGame>(objectResult.Value);
            Assert.Equal("TestGame", model.Identifier);
        }

        //[Fact]
        //public void Get_ReturnCardGame()
        //{
        //    var result = _controller.Get("TestGame");

        //    var objectResult = Assert.IsType<OkObjectResult>(result);
        //    var model = Assert.IsAssignableFrom<CardGame>(objectResult.Value);
        //    Assert.Equal("TestGame", model.Identifier);
        //}

        //[Fact]
        //public void Patch_UpdateGame()
        //{
        //    //var operation = new List<Operation> { new Operation("replace", nameof(CardGame.PlayerTurnIndex), "" };
        //    //var patchDocument = new JsonPatchDocument();
        //    var test = "[{ \"op\": \"replace\", \"path\": \"PlayerTurnIndex\", \"value\": \"5\" }]";
        //    var json = JsonConvert.DeserializeObject<JsonPatchDocument<CardGame>>(test);

        //    //var result = _controller.Patch("TestGame", json);

        //    //var objectResult = Assert.IsType<OkObjectResult>(result);
        //    //var model = Assert.IsAssignableFrom<CardGame>(objectResult.Value);
        //    //Assert.Equal("TestGame", model.Identifier);
        //}
    }
}

