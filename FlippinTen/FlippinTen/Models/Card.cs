using FlippinTen.Core.Entities.Enums;
using System;

namespace FlippinTen.Models
{
    public class Card : IEquatable<Card>
    {
        public Card(int id, int number, CardType cardType, bool isHidden)
        {
            ID = id;
            Number = number;
            CardType = cardType;
            IsHidden = isHidden;
            ImageUrl = GetCardImgUrl(number, cardType, isHidden);
        }

        private string GetCardImgUrl(int number, CardType cardType, bool isHidden)
        {
            return isHidden 
                ? ImageConstants.CardBack 
                : $"{cardType.Name.ToLower()}{number}.png";
        }

        public override bool Equals(object other)
        {
            if (other is null || !(other is Card card))
                return false;

            return Equals(card);
        }

        public bool Equals(Card other)
        {
            if (other is null)
                return false;

            return ID == other.ID;
        }

        public override int GetHashCode()
        {
            return 1213502048 + ID.GetHashCode();
        }

        public int ID { get; }
        public int Number { get; }
        public CardType CardType { get; }
        public string ImageUrl { get; }
        public bool IsHidden { get; }

        public override string ToString()
        {
            return $"{CardType.Name} {Number}";
        }
    }
}
