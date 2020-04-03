using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Entities
{
    public class CardGame
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public List<Player> Players { get; set; }
        public List<Card> DeckOfCards { get; set; }
        public List<Card> CardsOnTable { get; set; }
        public string CurrentPlayer { get; set; }
    }

    public class CardGameMove
    {
        public string GameIdentifier { get; set; }
        public Player Player { get; set; }
        public List<Card> DeckOfCards { get; set; }
        public List<Card> CardsOnTable { get; set; }
    }
}
