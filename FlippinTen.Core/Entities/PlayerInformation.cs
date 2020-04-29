//using Models.Entities;
using System;

namespace FlippinTen.Core.Entities
{
    public class PlayerInformation : IEquatable<PlayerInformation>
    {
        public PlayerInformation(string identifier)
        {
            Identifier = identifier;
        }
        public string Identifier { get; }
        public bool IsPlayersTurn { get; set; }

        public bool Equals(PlayerInformation other)
        {
            if (other == null || string.IsNullOrEmpty(other.Identifier))
            {
                return false;
            }

            return other.Identifier == Identifier;
        }
        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }
}
