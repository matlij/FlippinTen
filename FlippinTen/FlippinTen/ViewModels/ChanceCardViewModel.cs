using FlippinTen.Models;
using Xamarin.Forms;

namespace FlippinTen.ViewModels
{
    public class ChanceCardViewModel : BaseViewModel
    {
        private string _cardImg;
        private readonly string _cardImageUrl;

        public string CardImg
        {
            get { return _cardImg; }
            set { SetProperty(ref _cardImg, value); }
        }

        public Command TappedCardCommand { get; }

        public ChanceCardViewModel(string cardImageUrl, string cardBackUrl = ImageConstants.CardBack)
        {
            _cardImageUrl = cardImageUrl;
            CardImg = cardBackUrl;
            TappedCardCommand = new Command(OnCardTapped);
        }

        private void OnCardTapped()
        {
            CardImg = _cardImageUrl;
        }
    }
}
