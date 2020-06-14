﻿using FlippinTen.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
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
            await _viewModel.ConnectToGame();

            base.OnAppearing();
        }

        private async void DeckOfCardsTapped(object sender, EventArgs e)
        {
            var game = await _viewModel.CardGame.GetGame();
            var viewModel = new ChanceCardViewModel(_viewModel.CardGame, game);
            var popup = new ChanceCardPage(viewModel);
            popup.OnCardPlayed += async (s, @event) => await _viewModel.UpdateGameBoard(@event.GameResult);
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
                if (!(sender is Image cardOnTable))
                    return;

                var result = await _viewModel.CardOnTableTapped();
                if (result.Result == Core.Entities.Enums.CardPlayResult.CardsFlipped)
                {
                    await Task.WhenAll(
                        cardOnTable.RotateTo(180, 1000),
                        cardOnTable.ScaleTo(0, 1000)
                        );

                    cardOnTable.Rotation = 0;
                    return;
                }
            }
        }
    }
}