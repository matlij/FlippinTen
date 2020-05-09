using FlippinTen.Core.Models.Information;
using System;

namespace FlippinTen.Core
{
    public class CardGameEventArgs : EventArgs
    {
        public CardGameEventArgs(GameResult gameResult)
        {
            GameResult = gameResult;
        }

        public GameResult GameResult { get; }
    }
}
