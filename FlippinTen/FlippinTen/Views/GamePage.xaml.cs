using FlippinTen.Core.Entities;
using FlippinTen.ViewModels;
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

        public GamePage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new GameViewModel(null);
        }

        protected async override void OnAppearing()
        {
            if (!_viewModel.Connected)
                await _viewModel.ConnectToGame();

            base.OnAppearing();
        }

        protected async override void OnDisappearing()
        {
            await _viewModel.Disconnect();

            base.OnDisappearing();
        }

        private async void DeckOfCardsTapped(object sender, EventArgs e)
        {
            const string PickUp = "Plocka upp";
            const string ChanceCard = "Chansa";
            const string Cancel = "Avbryt";

            var action = await DisplayActionSheet("Vilket drag vill du göra?", Cancel, null, ChanceCard, PickUp);
            if (action == Cancel || string.IsNullOrEmpty(action))
            {
                return;
            }
            else if (action == ChanceCard)
            {
                _viewModel.PlayChanceCard();
            }
            else if (action == PickUp)
            {
                _viewModel.PickUpCards();
            }
            else
            {
                await DisplayAlert($"Fel: {action}", "Nånting gick fel :(", "Stäng");
            }
        }
    }
}