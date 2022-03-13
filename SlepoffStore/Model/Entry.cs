using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Model
{
    public sealed class Entry
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public string Caption { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} CategoryId:{CategoryId} Cation:{Caption}";
        }
    }
}
