using FlippinTen.Core.Models;
using Models;

namespace FlippinTen.Core.Interfaces
{
    public interface ICardGameEngine
    {
        GamePlay Game { get; set; }
        Player Player { get; }
        bool PickUpCardsFromTable();
        bool PlayCard(int cardNr);
        GamePlayResult PlayChanceCard();
    }
}