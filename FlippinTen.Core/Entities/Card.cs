using FlippinTen.Core.Entities.Enums;
using System;

namespace FlippinTen.Core.Entities
{
    public class Card
    {
        private const int _cardsPerType = 13;

        public Card(int number, CardType cardType)
        {
            if (number < 1 || number > _cardsPerType)
            {
                throw new ArgumentException($"Card number must be between 1 - {_cardsPerType}");
            }
            Number = number;
            CardType = cardType;

            SetCardID(cardType, number);

            //ImageUrl = $"{CardName}.jpg";
            ImageUrl = "hearts1.png";
        }

        private void SetCardID(CardType cardType, int number)
        {
            switch (cardType)
            {
                case CardType.Hearts:
                    ID = number;
                    break;
                case CardType.Dimonds:
                    ID = number + _cardsPerType;
                    break;
                case CardType.Clubs:
                    ID = number + _cardsPerType*2;
                    break;
                case CardType.Spades:
                    ID = number + _cardsPerType*3;
                    break;
                default:
                    throw new ArgumentException($"Card type invalid: {cardType}");
            }
        }

        public int ID { get; private set; }
        public int Number { get; }
        public CardType CardType { get; }
        public string ImageUrl { get; }
        public string CardName
        {
            get
            {
                return $"{CardType}{Number}";
            }
        }
    }
}
