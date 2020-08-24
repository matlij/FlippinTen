using FlippinTen.Core.Entities;
using FlippinTen.Translations;
using NUnit.Framework;
using System.Linq;

namespace FlippinTenMobileTest
{
    public class EntityTranslationsTests
    {
        [Test]
        public void PlayerAsTableCards()
        {
            var player = new Player("test");
            player.CardsHidden.Add(new Card(1));
            player.CardsHidden.Add(new Card(2));
            player.CardsHidden.Add(new Card(3));
            player.CardsVisible.Add(new Card(4));
            player.CardsVisible.Add(new Card(5));
            player.CardsVisible.Add(new Card(6));

            var tableCards = player.AsTableCards().Select(c => c.ID).ToList();

            Assert.AreEqual(3, tableCards.Count);
            Assert.Contains(player.CardsVisible[0].ID, tableCards);
            Assert.Contains(player.CardsVisible[1].ID, tableCards);
            Assert.Contains(player.CardsVisible[2].ID, tableCards);
        }
    }
}