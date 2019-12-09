using Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Models
{
    public class Player
    {
        public Player(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        public IList<CardCollection> CardsOnHand { get; set; } = new List<CardCollection>();
        public IList<Card> CardsHidden { get; } = new List<Card>();
        public IList<Card> CardsVisible { get; } = new List<Card>();
    }
}
