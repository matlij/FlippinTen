using FlippinTen.Core.Entities;
using System;

namespace FlippinTen.Core
{
    public class CardPlayedEventArgs : EventArgs
    {
        public CardGame Game { get; set; }
    }
}
