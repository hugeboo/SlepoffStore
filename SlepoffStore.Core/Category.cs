using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Core
{
    public sealed class Category
    {
        public long Id { get; set; }
        public long SectionId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} SectionId:{SectionId} Name:{Name}";
        }
    }
}
