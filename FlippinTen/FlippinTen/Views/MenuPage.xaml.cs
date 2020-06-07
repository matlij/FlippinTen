using FlippinTen.Bootstrap;
using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FlippinTen.Core.Factories;
using FlippinTen.Models.Constants;

namespace FlippinTen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        private readonly MenuViewModel _viewModel;
        private readonly ICardGameService _gameService;

        public MenuPage()
        {
            InitializeComponent();

            _gameService = AppContainer.Resolve<ICardGameOnlineService>();
            BindingContext = _viewModel = new MenuViewModel(_gameService);
        }

        protected override void OnAppearing()
        {
            _viewModel.LoadGamesCommand.Execute(null);

            base.OnAppearing();
        }

        private async void OnListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tappedGame = e.Item as GameFlippinTen;

            var hubConnection = ServerHubConnectionFactory.Create(_gameService, online: true);
            var cardGame = new CardGame(_gameService, hubConnection, tappedGame.Identifier, tappedGame.Player.UserIdentifier);
            var gameView = new GameViewModel(cardGame, tappedGame);

            await Navigation.PushAsync(new GamePage(gameView));
        }

        private async void OnCreateGameClicked(object sender, EventArgs e)
        {
            var view = new CreateGameViewModel(_gameService, DatabaseConstants.PlayerName);

            await Navigation.PushAsync(new CreateGamePage(view));
        }
    }
}