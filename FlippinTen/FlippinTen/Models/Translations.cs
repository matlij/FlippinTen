using FlippinTen.Core.Entities;
using FlippinTen.ViewModels;

namespace FlippinTen.Models
{
    internal static class Translations
    {
        internal static CardView AsCardView(this Card card)
        {
            return new CardView
            {
                ID = card.ID,
                Number = card.Number,
                ImageUrl = card.ImageUrl,
                Selected = card.Selected
            };
        }
    }
}
