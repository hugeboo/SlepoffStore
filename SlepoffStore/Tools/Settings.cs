using SlepoffStore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Tools
{
    public static class Settings
    {
        public static bool StartWithWindows { get; set; }
        public static Font MainFont { get; set; }

        public static void Load()
        {
            using var repo = new Repository();
            StartWithWindows = repo["StartWithWindows"] == "true";
            MainFont = FontFromString(repo["MainFont"]);
        }

        public static void Save()
        {
            using var repo = new Repository();
            repo["StartWithWindows"] = StartWithWindows.ToString().ToLower();
            repo["MainFont"] = MainFont != null ? FontToString(MainFont) : null;
        }

        public static void ActualizeStartWithWindows()
        {
            const string regKey = "SlepoffStore";

            using var reg = Microsoft.Win32.Registry.CurrentUser
                .CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

            var regValue = reg.GetValue(regKey) as string;

            if (StartWithWindows)
            {
                var autorunCommandLine = $"{Application.ExecutablePath} /database: \"{Repository.DatabaseName}\"";
                if (regValue != autorunCommandLine) reg.SetValue(regKey, autorunCommandLine);
            }
            else
            {
                if (!string.IsNullOrEmpty(regValue)) reg.DeleteValue(regKey);
            }
        }

        public static string FontToString(Font font)
        {
            if (font == null) return string.Empty;
            return $"{font.FontFamily.Name}; {font.SizeInPoints}pt; {font.Style}";
        }

        public static Font FontFromString(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var splits = s.Split("; ");
            if (splits.Length != 3) return null;

            var fontFamily = FontFamily.Families.FirstOrDefault(f => f.Name == splits[0]);
            if (fontFamily == null) return null;

            if (!splits[1].EndsWith("pt")) return null;
            if (!float.TryParse(splits[1].Substring(0, splits[1].Length - 2), out float pt)) return null;

            if (!Enum.TryParse<FontStyle>(splits[2], out FontStyle fs)) return null;

            return new Font(fontFamily, pt, fs);
        }
    }
}
