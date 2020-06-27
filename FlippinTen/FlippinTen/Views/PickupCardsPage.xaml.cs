using FlippinTen.Core.Entities;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlippinTen.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickupCardsPage : PopupPage
    {
        public PickupCardsPage(IEnumerable<Card> cardsOnTable)
        {
            InitializeComponent();
            CardsOnTable = new ObservableCollection<Card>(cardsOnTable);
            CollectionSpan = CardsOnTable.Count <= 5 ? CardsOnTable.Count : 5; 
            BindingContext = this;
        }

        public ObservableCollection<Card> CardsOnTable { get; }
        public int CollectionSpan { get;  }
        public bool PickupCards { get; private set; }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            PickupCards = true;
            await PopupNavigation.Instance.PopAsync();
        }
    }
}