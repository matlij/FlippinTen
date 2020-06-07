using FlippinTen.Bootstrap;
using FlippinTen.Core;
using FlippinTen.Core.Factories;
using FlippinTen.Core.Interfaces;
using FlippinTen.Models.Constants;
using FlippinTen.ViewModels;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlippinTen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private async void PlayComputerButtonClicked(object sender, EventArgs e)
        {
            var gameService = AppContainer.Resolve<ICardGameOfflineService>();
            var game = await gameService.Add("ComputerGame", DatabaseConstants.PlayerName, new List<string> { "Computer" });

            var hubConnection = ServerHubConnectionFactory.Create(gameService, online: false);
            var cardGame = new CardGame(gameService, hubConnection, game.Identifier, game.Player.UserIdentifier);

            var computerPlayer = ComputerPlayerFactory.Create(gameService, hubConnection, game);
            var gameView = new GameViewModel(cardGame, game, computerPlayer);

            await Navigation.PushAsync(new GamePage(gameView));
        }

        private async void PlayFriendButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MenuPage());
        }
    }
}