using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Utilities;
using FlippinTen.ViewModels;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FlippinTen.MobileApp.Tests
{
    public class Tests
    {
        private GameViewModel _sut;
        private Player _player;

        [SetUp]
        public void Setup()
        {
            _player = new Player("TestPlayer");
            _player.AddCardsToHand(new[] { new Card(1), new Card(14) });

            var playerInfo = new List<PlayerInformation>
            {
                new PlayerInformation(_player.UserIdentifier),
                new PlayerInformation("Other")
            };

            var game = new CardGame("TestId", "Test", new Stack<Card>(), new Stack<Card>(), _player, playerInfo);
            var onlineGameService = new OnlineGameService(new Mock<ICardGameService>().Object, new Mock<IServerHubConnection>().Object, game);
            _sut = new GameViewModel(onlineGameService);
        }

        [Test]
        public void Test1()
        {
            var cardFirst = _player.CardsOnHand.First();
            var cardView = new CardView
            {
                ID = cardFirst.ID,
                Number = cardFirst.Number
            };
            _sut.ItemTappedCommand.Execute(cardView);
        }
    }
}