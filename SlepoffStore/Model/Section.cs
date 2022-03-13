using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Model
{
    public class Section
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} Name:{Name}";
        }
    }

    public sealed class SectionEx : Section
    {
        public Category[] Categories { get; set; } = new Category[0];
    }
}
