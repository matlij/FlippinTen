using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlippinTenTests
{
    public class CardGameTest
    {
        private Player _player;
        private Player _opponent;
        private CardCollection _dummyCardCollection1;
        private CardCollection _dummyCardCollection2;
        private CardCollection _dummyCardCollectionWithCardTwo;
        private CardCollection _dummyCardCollectionWithCardTen;
        private Card _dummyCard1;
        private Card _dummyCard2;
        private CardGame _sut;

        [TestInitialize]
        public void Initialize()
        {
            _dummyCard1 = new Card(3, CardType.Clubs);
            _dummyCard2 = new Card(4, CardType.Dimonds);

            _dummyCardCollection1 = new CardCollection { Cards = new List<Card> { _dummyCard1 } };
            _dummyCardCollection2 = new CardCollection { Cards = new List<Card> { _dummyCard2 } };
            _dummyCardCollectionWithCardTwo = new CardCollection { Cards = new List<Card> { new Card(2, CardType.Clubs) } };
            _dummyCardCollectionWithCardTen = new CardCollection { Cards = new List<Card> { new Card(10, CardType.Clubs) } };

            _player = new Player("TestPlayer");
            _opponent = new Player("TestOpponent");

            var cardGameUtilities = new CardUtilities();

            var players = new List<Player> { _player, _opponent };
            _sut = new CardGame("TestGameId", "TestGameName", players, cardGameUtilities.GetDeckOfCards(), new Stack<Card>(), players.First());
        }

        [TestMethod]
        public void UpdateGame()
        {
            var newDeck = new List<Card>
            {
                new Card(1, CardType.Clubs),
                new Card(2, CardType.Dimonds)
            };

            _sut.UpdateGame(null, newDeck, null);
        }
    }
}
