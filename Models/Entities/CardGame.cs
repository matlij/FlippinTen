using System.Collections.Generic;

namespace FlippinTen.Models.Entities
{
    public class CardGame
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public Stack<Card> DeckOfCards { get; set; }
        public Stack<Card> CardsOnTable { get; set; }
        public bool GameOver { get; set; }
        public string Winner { get; set; }
    }
}
