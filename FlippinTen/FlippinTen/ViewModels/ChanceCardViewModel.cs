using FlippinTen.Core;
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

        private readonly OnlineGameService _onlineGameService;

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

        public ChanceCardViewModel(OnlineGameService onlineGameService, string cardBackUrl = ImageConstants.CardBack)
        {
            _onlineGameService = onlineGameService ?? throw new ArgumentNullException(nameof(onlineGameService));
            ChanceCard = cardBackUrl;
            TopCard = onlineGameService.Game.CardsOnTable.Count > 0 
                ? onlineGameService.Game.CardsOnTable.Peek().ImageUrl
                : null;
        }

        public async Task<GameResult> PlayChanceCard()
        {
            IsBusy = true;

            var result = await _onlineGameService.Play(g => g.PlayChanceCard());

            IsBusy = false;

            return result;
        }
    }
}
