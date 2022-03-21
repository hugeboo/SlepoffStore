using System;
using System.Collections.Generic;
using System.Text;

namespace SlepoffStore.Core
{
    public sealed class Device
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} Name:{Name}";
        }
    }
}
