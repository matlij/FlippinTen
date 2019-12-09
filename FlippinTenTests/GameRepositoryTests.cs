using FlippinTenWeb.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlippinTenTests
{
    [TestClass]
    public class GameRepositoryTests
    {
        private IGameRepository _sut;

        [TestInitialize]
        public void Initialize()
        {
            _sut = new GameRepository();
        }

        [TestMethod]
        public void UpdateCards_ShouldUpdate()
        {
            //Arrange

            //Act

            //Assert
        }
    }
}
