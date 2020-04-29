using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Services;
using FlippinTen.Utilities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using dto = Models.Entities;

namespace FlippinTenTests.Core
{
    [TestClass]
    public class CardGameServiceTest
    {
        private Mock<IGenericRepository> _repository;
        private CardGameUtilities _cardGameUtilities;
        private ICardGameService _sut;

        [TestInitialize]
        public void Initialize()
        {
            _repository = new Mock<IGenericRepository>();
            _cardGameUtilities = new CardGameUtilities(new CardUtilities());
            _sut = new CardGameService(_repository.Object, _cardGameUtilities);
        }

        [TestMethod]
        public void Update()
        {
            var game = _cardGameUtilities.CreateGame("TestGame", new List<string> { "Player", "Opponent" });

            _sut.Update(game);

            _repository.Verify(r => r.PatchAsync(It.IsAny<string>(), It.IsAny<JsonPatchDocument<dto.CardGame>>()));
        }
    }
}
