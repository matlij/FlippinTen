using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Events
{
    public class PlayerJoinedEventArgs : EventArgs
    {
        public string GameName { get; set; }
        public string PlayerName { get; set; }
    }
}
