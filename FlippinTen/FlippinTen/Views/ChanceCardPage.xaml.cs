using FlippinTen.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlippinTen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChanceCardPage : PopupPage
    {
        private readonly ChanceCardViewModel _viewModel;
        private bool _complete;

        public bool PlayChanceCard { get; private set; } = false;

        public ChanceCardPage(ChanceCardViewModel viewModel)
        {
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
            PlayChanceCard = true;
            if (_complete)
                await PopupNavigation.Instance.PopAsync();

            await ChanceCardImg.FadeTo(0, 500);
            var result = _viewModel.PlayChanceCard();
            await ChanceCardImg.FadeTo(1, 500);

            UpdateChanceCardButton(result);

            _complete = true;
        }

        private void UpdateChanceCardButton(bool result)
        {
            ChanceCardButton.Text = result
                ? "Chansning lyckades!"
                : "Chansing misslyckades...";

            ChanceCardButton.BackgroundColor = result
                ? Color.Green
                : Color.Red;
        }
    }
}