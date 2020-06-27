using FlippinTen.Core.Entities.Enums;
using FlippinTen.ViewModels;
using Rg.Plugins.Popup.Events;
using Rg.Plugins.Popup.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlippinTen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        private GameViewModel _viewModel;
        private bool _runCardTwoPlayedAnimation = false;

        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        protected async override void OnAppearing()
        {
            PopupNavigation.Instance.Popping += OnPopUpPopped;

            await _viewModel.ConnectToGame();

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            PopupNavigation.Instance.Popping -= OnPopUpPopped;

            base.OnDisappearing();
        }

        private async void DeckOfCardsTapped(object sender, EventArgs e)
        {
            var game = await _viewModel.CardGame.GetGame();
            var viewModel = new ChanceCardViewModel(game);
            var popup = new ChanceCardPage(viewModel);

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void CardsOnTableTapped(object sender, EventArgs e)
        {
            if (_viewModel.SelectedCards.Count == 0)
            {
                var game = await _viewModel.CardGame.GetGame();
                var popup = new PickupCardsPage(game.CardsOnTable.ToList());
                await PopupNavigation.Instance.PushAsync(popup);
            }
            else
            {
                await _viewModel.CardOnTableTapped();
            }
        }

        private async void OnPopUpPopped(object sender, PopupNavigationEventArgs e)
        {
            if (e.Page is ChanceCardPage chanceCardPage && chanceCardPage.PlayChanceCard)
            { 
                _viewModel.PlayChanceCard();
            }
            else if (e.Page is PickupCardsPage pickupCardsPage && pickupCardsPage.PickupCards)
            {
                await _viewModel.PickUpCards();
            }
        }

        private async void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_viewModel.LastGameResult))
                return;

            var result = _viewModel.LastGameResult.Result;

            switch (result)
            {
                case CardPlayResult.Unknown:
                case CardPlayResult.Invalid:
                    StartInvalidAnimation();
                    break;
                case CardPlayResult.Succeded:
                    await StartCardPlayedAnimation();
                    break;
                case CardPlayResult.CardsFlipped:
                    await CardsFlippedAnimation();
                    break;
                case CardPlayResult.CardTwoPlayed:
                    StartCardTwoAnimation();
                    break;
            }
        }

        private async Task StartCardPlayedAnimation()
        {
            if (!_runCardTwoPlayedAnimation)
            {
                await TopCardOnTableImage.ScaleTo(0.8, 150, Easing.Linear);
                await TopCardOnTableImage.ScaleTo(1, 150, Easing.Linear);
            }

            _runCardTwoPlayedAnimation = false;
        }

        private void StartInvalidAnimation()
        {
            var animation = new Animation
                {
                    { 0, 0.125, new Animation (v => TopCardOnTableImage.TranslationX = v, 0, -13) },
                    { 0.125, 0.250, new Animation (v => TopCardOnTableImage.TranslationX = v, -13, 13) },
                    { 0.250, 0.375, new Animation (v => TopCardOnTableImage.TranslationX = v, 13, -11) },
                    { 0.375, 0.5, new Animation (v => TopCardOnTableImage.TranslationX = v, -11, 11) },
                    { 0.5, 0.625, new Animation (v => TopCardOnTableImage.TranslationX = v, 11, -7) },
                    { 0.625, 0.75, new Animation (v => TopCardOnTableImage.TranslationX = v, -7, 7) },
                    { 0.75, 0.875, new Animation (v => TopCardOnTableImage.TranslationX = v, 7, -5) },
                    { 0.875, 1, new Animation (v => TopCardOnTableImage.TranslationX = v, -5, 0) }
                };
            animation.Commit(this, "ShakeIt", length: 500, easing: Easing.Linear);
        }

        private void StartCardTwoAnimation()
        {
            if (_runCardTwoPlayedAnimation)
                return;

            _runCardTwoPlayedAnimation = true;

            var animation = new Animation
                {
                    { 0, 0.5, new Animation (v => TopCardOnTableImage.Scale = v, 1, 0.8, Easing.SinInOut) },
                    { 0.5, 1, new Animation (v => TopCardOnTableImage.Scale = v, 0.8, 1, Easing.SinInOut) }
                };
            animation.Commit(this, "ScaleIt", length: 2000, easing: Easing.Linear, repeat: () => _runCardTwoPlayedAnimation);
        }

        private async Task CardsFlippedAnimation()
        {
            await Task.WhenAll(
                TopCardOnTableImage.RotateYTo(720, 1000, Easing.Linear),
                TopCardOnTableImage.ScaleTo(0, 1000, Easing.Linear)
                );

            _viewModel.TopCardOnTable = null;
            TopCardOnTableImage.RotationY = 0;
            TopCardOnTableImage.Scale = 1;
        }
    }
}