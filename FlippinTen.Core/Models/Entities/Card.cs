using FlippinTen.Core.Entities.Enums;
using System;

namespace FlippinTen.Core.Entities
{
    public class Card : IEquatable<Card>, IComparable<Card>
    {
        private const int _cardsPerType = 13;

        public Card(int cardId)
        {
            var maxCardId = _cardsPerType * CardType.GetList().Count;
            if (cardId < 1 || cardId > maxCardId)
            {
                throw new ArgumentException($"Card ID must be between 1 - {maxCardId}");
            }

            foreach (var cardType in CardType.GetList())
            {
                var cardTypeFirstId = _cardsPerType * (cardType.Value - 1);
                var cardTypeLastId = _cardsPerType * cardType.Value;
                if (cardId > cardTypeFirstId && cardId <= cardTypeLastId)
                {
                    CardType = cardType;
                    Number = cardId + 1 - cardTypeFirstId;
                    break;
                }
            }

            ID = cardId;
        }

        public Card(int number, CardType cardType)
        {
            var maxNumber = _cardsPerType + 1;
            if (number < 1 || number > maxNumber)
            {
                throw new ArgumentException($"Card number must be between 1 - {maxNumber}");
            }

            Number = number;
            CardType = cardType;
            ID = number + (cardType.Value - 1) * maxNumber;
        }

        public int ID { get; }
        public int Number { get; }
        public CardType CardType { get; }
        public override bool Equals(object other)
        {
            if (other is null || !(other is Card otherCard))
                return false;

            return Equals(otherCard);
        }

        public bool Equals(Card other)
        {
            if (other is null)
            {
                return false;
            }

            return other.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public int CompareTo(Card other)
        {
            if (other == null)
                return 1;

            return ID.CompareTo(other.ID);
        }
    }
}
