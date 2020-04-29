using System.Collections.Generic;

namespace Models.Entities
{
    public class Player
    {
        public string UserIdentifier { get; set; }
        public bool IsConnected { get; set; }
        public IList<Card> CardsOnHand { get; set; } = new List<Card>();
        public IList<Card> CardsHidden { get; set; } = new List<Card>();
        public IList<Card> CardsVisible { get; set; } = new List<Card>();
        public bool IsPlayersTurn { get; set; }
    }
}
