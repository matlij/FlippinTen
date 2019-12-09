using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Events
{
    public class CardPlayedEventArgs : EventArgs
    {
        public GamePlay Game { get; set; }
    }
}
