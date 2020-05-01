using FlippinTen.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlippinTenTests.Core
{
    [TestClass]
    public class CardTest
    {
        [TestMethod]
        [DataRow(13)]
        [DataRow(26)]
        [DataRow(39)]
        [DataRow(52)]
        public void Card_CreateAce_NumberShouldBeFourteen(int id)
        {
            var card = new Card(id);
            var aceNumber = 14;

            Assert.AreEqual(aceNumber, card.Number);
        }
    }
}
