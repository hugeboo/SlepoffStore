using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Core
{
    public sealed class Entry
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public DateTime CreationDate { get; set; }
        public EntryColor Color { get; set; }
        public string Caption { get; set; }
        public string? Text { get; set; }
        public DateTime? Alarm { get; set; }
        public bool AlarmIsOn { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} CategoryId:{CategoryId} Date:{CreationDate} Cation:{Caption}";
        }
    }

    public enum EntryColor
    {
        Black,
        Blue,
        Green,
        Cyan,
        Red,
        Magenta,
        Yellow,
        White
    }
}
