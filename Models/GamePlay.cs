using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class GamePlay
    {
        public GamePlay(List<Player> players)
        {
            if (players is null) throw new ArgumentNullException(nameof(players));

            Players = players;

            if (players.Count > 0)
            {
                PlayerTurnIdentifier = players.First().Identifier;
            }
        }

        public string Identifier { get; set; }
        public string Name { get; set; }
        public List<Player> Players { get; set; }
        public Stack<Card> DeckOfCards { get; set; }
        public Stack<Card> CardsOnTable { get; set; }
        public string PlayerTurnIdentifier { get; set; }
    }
}
