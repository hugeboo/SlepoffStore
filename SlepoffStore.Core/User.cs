using System;
using System.Collections.Generic;
using System.Text;

namespace SlepoffStore.Core
{
    public sealed class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Comments { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} Name:{Name}";
        }
    }
}
