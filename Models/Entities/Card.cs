using FlippinTen.Models.Enums;
using System;

namespace FlippinTen.Models.Entities
{
    public class Card
    {
        public int ID { get; set; }
        public int Number { get; set; }
        public CardType CardType { get; set; }
        public string ImageUrl { get; set; }
    }
}
