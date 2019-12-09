using FlippinTen.Bootstrap;
using FlippinTen.Core.Interfaces;
using FlippinTen.ViewModels;
using Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlippinTen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateGamePage : ContentPage
    {
        private readonly CreateGameViewModel _viewModel;

        public CreateGamePage(CreateGameViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        public CreateGamePage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new CreateGameViewModel(AppContainer.Resolve<IGameMenuService>(), DatabaseConstants.PlayerName);
        }

        private async void OnCreateGameClicked(object sender, EventArgs e)
        {
            await _viewModel.CreateGame(name.Text, opponent.Text);

            await Navigation.PopAsync();
        }
    }
}