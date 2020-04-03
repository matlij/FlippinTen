using System;
using System.Collections.Generic;
using System.Text;

namespace FlippinTen.Core
{
    public class PlayerJoinedEventArgs : EventArgs
    {
        public string GameName { get; set; }
        public string UserIdentifier { get; set; }
    }
}
