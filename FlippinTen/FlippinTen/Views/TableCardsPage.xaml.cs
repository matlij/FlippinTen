using FlippinTen.Core.Entities;
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
    public partial class TableCardsPage : PopupPage
    {
        public TableCardsPage(TableCardsViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}