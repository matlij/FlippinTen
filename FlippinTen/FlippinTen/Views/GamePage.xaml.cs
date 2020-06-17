using FlippinTen.Core.Entities.Enums;
using FlippinTen.ViewModels;
using Rg.Plugins.Popup.Events;
using Rg.Plugins.Popup.Services;
using System;
using System.ComponentModel;
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
                var reply = await DisplayAlert("Plocka upp kort", "Är du säker på att du vill plocka upp kort?", "Ja!", "Nej!");
                if (!reply)
                    return;

                await _viewModel.PickUpCards();
            }
            else
            {
                await _viewModel.CardOnTableTapped();
            }
        }

        private void OnPopUpPopped(object sender, PopupNavigationEventArgs e)
        {
            if (!(e.Page is ChanceCardPage chanceCardPage))
                return;

            if (chanceCardPage.PlayChanceCard)
            {
                _viewModel.PlayChanceCard();
            }
        }

        private async void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_viewModel.LastGameResult))
                return;
            if (TopCardOnTableImage.Opacity != 1)
                TopCardOnTableImage.Opacity = 1;

            var result = _viewModel.LastGameResult.Result;

            if (result == CardPlayResult.CardsFlipped)
            {
                await Task.WhenAll(
                    TopCardOnTableImage.RotateTo(180, 750, Easing.SinIn),
                    TopCardOnTableImage.ScaleTo(0, 750, Easing.SinIn)
                    );

                TopCardOnTableImage.Rotation = 0;
                TopCardOnTableImage.Opacity = 0;
                TopCardOnTableImage.Scale = 1;
            }
            else if (result == CardPlayResult.CardTwoPlayed)
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
            else if (result == CardPlayResult.Invalid)
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
            else if (result == CardPlayResult.Succeded)
            {
                if (!_runCardTwoPlayedAnimation)
                {
                    await TopCardOnTableImage.ScaleTo(0.8, 150, Easing.Linear);
                    await TopCardOnTableImage.ScaleTo(1, 150, Easing.Linear);
                }

                _runCardTwoPlayedAnimation = false;
            }
        }
    }
}