using FlippinTen.Core.Entities.Enums;
using System;

namespace FlippinTen.Core.Entities
{
    public class Card : IEquatable<Card>
    {
        private const int _cardsPerType = 13;

        public Card(int cardId)
        {
            var maxCardId = _cardsPerType * CardType.GetList().Count;
            if (cardId < 1 || cardId > maxCardId)
            {
                throw new ArgumentException($"Card number must be between 1 - {maxCardId}");
            }

            foreach (var cardType in CardType.GetList())
            {
                var cardTypeFirstId = _cardsPerType * (cardType.Value - 1);
                var cardTypeLastId = _cardsPerType * cardType.Value;
                if (cardId > cardTypeFirstId && cardId <= cardTypeLastId)
                {
                    CardType = cardType;
                    Number = cardId - cardTypeFirstId;
                    break;
                }
            }

            ID = cardId;
            ImageUrl = $"spades{Number}.png";
        }
        public Card(int number, CardType cardType)
        {
            if (number < 1 || number > _cardsPerType)
            {
                throw new ArgumentException($"Card number must be between 1 - {_cardsPerType}");
            }

            Number = number;
            CardType = cardType;
            ID = number + (cardType.Value - 1) * _cardsPerType;
            ImageUrl = $"spades{number}.png";
        }

        public int ID { get; }
        public int Number { get; }
        public CardType CardType { get; }
        public string ImageUrl { get; }
        public bool Selected { get; set; }

        public bool Equals(Card other)
        {
            return other.ID == ID;
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public override string ToString()
        {
            return $"{CardType.Name} {Number}";
        }
    }
}
