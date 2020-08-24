using FlippinTen.Models;
using System.Collections.Generic;

namespace FlippinTen.ViewModels
{
    public class TableCardsViewModel : BaseViewModel
    {
        public TableCardsViewModel(IEnumerable<Card> tableCards)
        {
            TableCards = tableCards;
        }

        public IEnumerable<Card> TableCards { get; }
    }
}
