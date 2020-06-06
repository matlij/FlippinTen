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
using System.Linq;

namespace FlippinTen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        private readonly MenuViewModel _viewModel;
        private readonly bool _playOnline;
        private readonly ICardGameService _gameService;

        public MenuPage(bool playOnline)
        {
            InitializeComponent();

            _playOnline = playOnline;

            _gameService = playOnline
                ? AppContainer.Resolve<ICardGameOnlineService>()
                : (ICardGameService)AppContainer.Resolve<ICardGameOfflineService>();
            BindingContext = _viewModel = new MenuViewModel(_gameService);
        }

        protected override void OnAppearing()
        {
            if (_viewModel.OnGoingGames.Count == 0)
                _viewModel.LoadGamesCommand.Execute(null);

            base.OnAppearing();
        }

        private async void OnListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tappedGame = e.Item as GameFlippinTen;

            var hubConnection = ServerHubConnectionFactory.Create(_gameService, _playOnline);
            var cardGame = new CardGame(_gameService, hubConnection, tappedGame.Identifier, tappedGame.Player.UserIdentifier);

            GameViewModel gameView;
            if (_playOnline)
            {
                gameView = new GameViewModel(cardGame, tappedGame);
            }
            else
            {
                var computerPlayer = ComputerPlayerFactory.Create(_gameService, hubConnection, tappedGame);
                gameView = new GameViewModel(cardGame, tappedGame, computerPlayer);
            }

            await Navigation.PushAsync(new GamePage(gameView));
        }

        private async void OnCreateGameClicked(object sender, EventArgs e)
        {
            var view = new CreateGameViewModel(_gameService, DatabaseConstants.PlayerName);

            await Navigation.PushAsync(new CreateGamePage(view));
        }
    }
}