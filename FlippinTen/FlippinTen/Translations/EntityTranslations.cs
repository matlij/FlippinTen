using FlippinTen.Core.Entities;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FlippinTenMobileTest")]
namespace FlippinTen.Translations
{
    internal static class EntityTranslations
    {
        public static Card AsCard(this Models.Card card)
        {
            return new Card(card.Number, card.CardType);
        }

        public static Models.Card AsCard(this Card card, bool isHidden)
        {
            return new Models.Card(card.ID, card.Number, card.CardType, isHidden);
        }

        public static IList<Models.Card> AsTableCards(this Player player)
        {
            var tableCards = new List<Models.Card>();
            foreach (var card in player.CardsVisible)
            {
                tableCards.Add(card.AsCard(false));
            }
            foreach (var card in player.CardsHidden)
            {
                if (tableCards.Count >= 3)
                    break;

                tableCards.Add(card.AsCard(true));
            }

            return tableCards;
        }
    }
}
