using Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Models.Entities
{
    public class Player
    {
        public string UserIdentifier { get; set; }
        public bool IsConnected { get; set; }
        public IList<CardCollection> CardsOnHand { get; set; } = new List<CardCollection>();
        public IList<Card> CardsHidden { get; set; } = new List<Card>();
        public IList<Card> CardsVisible { get; set; } = new List<Card>();
        public bool IsPlayersTurn { get; set; }
    }
}
