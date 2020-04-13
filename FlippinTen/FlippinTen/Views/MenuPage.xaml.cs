﻿using FlippinTen.Bootstrap;
using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Services;
using FlippinTen.Core.Utilities;
using FlippinTen.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using Models.Constants;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlippinTen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        private readonly MenuViewModel _viewModel;

        public MenuPage()
        {
            InitializeComponent();

            if (DesignMode.IsDesignModeEnabled)
            {
                // Previewer only code  
                BindingContext = _viewModel = new MenuViewModel();
            }
            else
            {
                BindingContext = _viewModel = new MenuViewModel(AppContainer.Resolve<IGameMenuService>());
            }
        }

        protected override void OnAppearing()
        {
            if (!DesignMode.IsDesignModeEnabled)
            {
                if (_viewModel.OnGoingGames.Count == 0)
                    _viewModel.LoadGamesCommand.Execute(null);
            }

            base.OnAppearing();
        }

        private async void OnListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tappedItem = e.Item as CardGame;

            var hubConnection = new ServerHubConnection(new HubConnectionBuilder(), UriConstants.BaseUri + UriConstants.GameHub);
            var onlineGameService = new OnlineGameService(AppContainer.Resolve<ICardGameService>(), hubConnection, tappedItem);
            var gameView = new GameViewModel(onlineGameService);

            await Navigation.PushAsync(new GamePage(gameView));
        }

        private async void OnCreateGameClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateGamePage());

            //_viewModel.LoadGamesCommand.Execute(null);
        }
    }
}