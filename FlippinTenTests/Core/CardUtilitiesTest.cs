using FlippinTen.Core.Entities;
using FlippinTen.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlippinTenTests
{
    [TestClass]
    public class CardUtilitiesTest
    {
        private CardUtilities _sut;
        private const int CardsCount = 52;

        [TestInitialize]
        public void Initialize()
        {
            _sut = new CardUtilities();
        }

        [TestMethod]
        public void CardUtilities_GetDeckOfCards_ShouldBeCorrectCount()
        {
            var cards = _sut.GetDeckOfCards();

            Assert.IsNotNull(cards);
            Assert.AreEqual(cards.Count, CardsCount);
        }

        [TestMethod]
        public void CardUtilities_GetDeckOfCards_ShouldIncludeAllCards()
        {
            var cards = _sut.GetDeckOfCards();

            for (var i = 1; i <= CardsCount; i++)
            {
                var card = new Card(i);
                cards.Contains(card);
            }
        }
    }
}
