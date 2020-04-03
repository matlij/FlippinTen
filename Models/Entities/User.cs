using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Entities
{
    public class User
    {
        public User() { }
        public User(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; }
        public string Name { get; set; }
    }
}
