using FlippinTen.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlippinTen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        private GameViewModel _viewModel;

        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;
        }

        protected async override void OnAppearing()
        {
            if (!_viewModel.Connected)
                await _viewModel.ConnectToGame();

            base.OnAppearing();
        }

        private async void DeckOfCardsTapped(object sender, EventArgs e)
        {
            var viewModel = new ChanceCardViewModel(_viewModel.OnlineGameService);
            var popup = new ChanceCardPage(viewModel);
            popup.OnCardPlayed += (s, @event) => _viewModel.UpdateGameBoard(@event.GameResult);
            await PopupNavigation.Instance.PushAsync(popup);
        }
    }
}