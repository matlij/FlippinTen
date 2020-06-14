using FlippinTen.Core.Entities;
using FlippinTen.Models;
using System;

namespace FlippinTen.ViewModels
{
    public class ChanceCardViewModel : BaseViewModel
    {
        private readonly GameFlippinTen _game;

        private string _chanceCard;
        private string _topCard;
        private bool _buttonEnabled;

        public string ChanceCard
        {
            get { return _chanceCard; }
            set { SetProperty(ref _chanceCard, value); }
        }
        public string TopCard
        {
            get { return _topCard; }
            set { SetProperty(ref _topCard, value); }
        }
        public bool ButtonEnabled
        {
            get { return _buttonEnabled; }
            set { SetProperty(ref _buttonEnabled, value); }
        }

        public ChanceCardViewModel(GameFlippinTen game, string cardBackUrl = ImageConstants.CardBack)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            ButtonEnabled = true;
            ChanceCard = cardBackUrl;
            TopCard = game.CardsOnTable.Count > 0 
                ? game.CardsOnTable.Peek().ImageUrl
                : null;
        }

        public bool PlayChanceCard()
        {
            IsBusy = true;

            ButtonEnabled = false;

            var chanceCard = _game.DeckOfCards.Peek();
            var canPlayCard = _game.CanPlayCard(chanceCard);
            ChanceCard = chanceCard.ImageUrl;

            IsBusy = false;

            return canPlayCard;
        }
    }
}
