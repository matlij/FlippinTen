using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class CardCollection
    {
        public int CardNr 
        {
            get
            {
                return Cards.Count > 0
                    ? Cards.FirstOrDefault().Number
                    : 0;
            }
        }

        public string CardNames
        {
            get
            {
                return Cards.Count > 0
                    ? string.Join(", ", Cards.Select(c => c.CardName))
                    : string.Empty;
            }
        }

        public List<Card> Cards { get; set; } = new List<Card>();
        public string ImageUrl { get; } = "acespades.png";
    }
}
