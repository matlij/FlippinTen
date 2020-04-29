using FlippinTen.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlippinTenTests
{
    [TestClass]
    public class CardUtilitiesTest
    {
        private CardUtilities _sut;

        [TestInitialize]
        public void Initialize()
        {
            _sut = new CardUtilities();
        }

        [TestMethod]
        public void CardUtilities_GetDeckOfCards()
        {
            var cards = _sut.GetDeckOfCards();

            Assert.IsNotNull(cards);
            Assert.AreEqual(cards.Count, 52);
        }
    }
}
