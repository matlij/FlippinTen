using FlippinTen.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlippinTen.Models.Information
{
    public class GameResult
    {
        public string GameIdentifier { get; set; }
        public string UserIdentifier { get; set; }
        public int Result { get; set; }
        public IEnumerable<Card> Cards { get; set; }
    }
}
