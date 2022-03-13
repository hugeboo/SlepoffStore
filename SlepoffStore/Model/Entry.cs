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
        public DateTime CreationDate { get; set; }
        public EntryColor Color { get; set; }
        public string Caption { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} CategoryId:{CategoryId} Cation:{Caption}";
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

    public static class EntryColorExtension
    {
        public static Color ToColor(this EntryColor ec)
        {
            switch (ec)
            {
                case EntryColor.Black:
                    return Color.Black;

                case EntryColor.Blue:
                    return Color.Blue;

                case EntryColor.Green:
                    return Color.Green;

                case EntryColor.Cyan:
                    return Color.Cyan;

                case EntryColor.Red:
                    return Color.Red;

                case EntryColor.Magenta:
                    return Color.Magenta;

                case EntryColor.Yellow:
                    return Color.Yellow;

                case EntryColor.White:
                    return Color.White;

                default:
                    throw new Exception("Unknown color");
            }
        }

        public static Color GetForeColor(this EntryColor ec)
        {
            switch (ec)
            {
                case EntryColor.Black:
                case EntryColor.Blue:
                case EntryColor.Green:
                case EntryColor.Red:
                    return Color.White;

                case EntryColor.Cyan:
                case EntryColor.Magenta:
                case EntryColor.Yellow:
                case EntryColor.White:
                    return Color.Black;

                default:
                    throw new Exception("Unknown color");
            }
        }
    }
}
