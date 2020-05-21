using System.Collections.Generic;
using System.Linq;

namespace FlippinTen.Core.Entities.Enums
{
    public class CardType
    {
        public static CardType Hearts { get; } = new CardType(1, "Hearts");
        public static CardType Dimonds { get; } = new CardType(2, "Dimonds");
        public static CardType Cloves { get; } = new CardType(3, "Cloves");
        public static CardType Spades { get; } = new CardType(4, "Spades");

        public string Name { get; private set; }
        public int Value { get; private set; }

        private CardType(int val, string name)
        {
            Value = val;
            Name = name;
        }

        public static List<CardType> GetList()
        {
            var cardTypes = new CardType[] { Hearts, Dimonds, Cloves, Spades };
            return new List<CardType>(cardTypes);
        }

        public static CardType FromValue(int value)
        {
            return GetList().Single(c => c.Value == value);
        }
    }
}
