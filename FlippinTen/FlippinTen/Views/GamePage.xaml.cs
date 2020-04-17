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
            if (!DesignMode.IsDesignModeEnabled)
            {
                if (!_viewModel.Connected)
                    await _viewModel.ConnectToGame();
            }

            base.OnAppearing();
        }

        private async void OnCardOnHandTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is CardCollection cardCollection))
                return;

            await _viewModel.PlayCard(cardCollection);
        }

        private void OnChanceCardClicked(object sender, EventArgs e)
        {
            _viewModel.PlayChanceCard();
        }

        private void OnPickUpCardClicked(object sender, EventArgs e)
        {
            //_viewModel.PlayChanceCard();
        }
    }
}