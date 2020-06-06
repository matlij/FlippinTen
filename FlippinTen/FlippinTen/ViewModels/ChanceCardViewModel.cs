using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Models.Information;
using FlippinTen.Models;
using System;
using System.Threading.Tasks;

namespace FlippinTen.ViewModels
{
    public class ChanceCardViewModel : BaseViewModel
    {
        private string _chanceCard;
        private string _topCard;

        private readonly ICardGame _cardGame;

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

        public ChanceCardViewModel(ICardGame cardGame, GameFlippinTen game, string cardBackUrl = ImageConstants.CardBack)
        {
            _cardGame = cardGame ?? throw new ArgumentNullException(nameof(cardGame));
            ChanceCard = cardBackUrl;
            TopCard = game.CardsOnTable.Count > 0 
                ? game.CardsOnTable.Peek().ImageUrl
                : null;
        }

        public async Task<GameResult> PlayChanceCard()
        {
            IsBusy = true;

            var result = await _cardGame.Play(g => g.PlayChanceCard());

            IsBusy = false;

            return result;
        }
    }
}
