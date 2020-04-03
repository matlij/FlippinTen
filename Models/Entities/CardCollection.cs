﻿using System.Collections.Generic;
using System.Linq;

namespace Models.Entities
{
    public class CardCollection
    {
        public List<Card> Cards { get; set; } = new List<Card>();
        public string ImageUrl { get; set; }
    }
}
