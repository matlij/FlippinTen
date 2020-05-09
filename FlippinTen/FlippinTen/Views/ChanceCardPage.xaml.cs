using FlippinTen.ViewModels;
using Rg.Plugins.Popup.Pages;
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
    public partial class ChanceCardPage : PopupPage
    {
        private readonly ChanceCardViewModel _viewModel;

        public ChanceCardPage(ChanceCardViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            BackgroundColor = new Color(0, 0, 0, 0.4);

            base.OnAppearing();
        }
    }
}