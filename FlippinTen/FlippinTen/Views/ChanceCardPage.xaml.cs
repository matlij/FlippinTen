using FlippinTen.Core;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlippinTen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChanceCardPage : PopupPage
    {
        private readonly ChanceCardViewModel _viewModel;
        private bool _complete;
        private bool _buttonEnabled;

        public event EventHandler<CardGameEventArgs> OnCardPlayed;

        public ChanceCardPage(ChanceCardViewModel viewModel)
        {
            _buttonEnabled = true;

            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            BackgroundColor = new Color(0, 0, 0, 0.65);

            base.OnAppearing();
        }

        private async void OnTakeChanceCardClicked(object sender, EventArgs e)
        {
            if (!_buttonEnabled)
                return;
            _buttonEnabled = false;

            if (_complete)
                await PopupNavigation.Instance.PopAsync();

            var result = await _viewModel.PlayChanceCard();

            await ChanceCardImg.FadeTo(0, 500);
            _viewModel.ChanceCard = result.Cards.FirstOrDefault()?.ImageUrl;
            await ChanceCardImg.FadeTo(1, 1000);

            if (result.Result == CardPlayResult.ChanceSucceded)
            {
                ChanceCardButton.Text = "Chansning lyckades!";
                ChanceCardButton.BackgroundColor = Color.Green;
            }
            else
            {
                ChanceCardButton.Text = "Chansing misslyckades...";
                ChanceCardButton.BackgroundColor = Color.Red;
            }

            OnCardPlayed?.Invoke(this, new CardGameEventArgs(result));

            _complete = true;
            _buttonEnabled = true;
        }
    }
}