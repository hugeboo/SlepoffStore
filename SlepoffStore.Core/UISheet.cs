using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Core
{
    public sealed class UISheet
    {
        public long Id { get; set; }
        public long EntryId { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} EntryId:{EntryId}";
        }
    }
}
