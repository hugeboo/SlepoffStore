using SlepoffStore.Core;
using SlepoffStore.Repository;
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
        public static string AlarmRingtone { get; set; }
        public static Font MainFont { get; set; }

        public static string ApplicationPath => Application.ExecutablePath;

        public static async Task<bool> Load()
        {
            try
            {
                using var repo = Program.CreateRepository();
                StartWithWindows = await repo.GetValue("StartWithWindows") == "true";
                AlarmRingtone = await repo.GetValue("AlarmRingtone");
                MainFont = FontFromString(await repo.GetValue("MainFont"));
                return true;
            }
            catch (RemoteException ex)
            {
                ExceptionForm.ShowConnectingError(ex);
                return false;
            }
        }

        public static async Task Save()
        {
            try
            {
                using var repo = Program.CreateRepository();
                await repo.SetValue("StartWithWindows", StartWithWindows.ToString().ToLower());
                await repo.SetValue("AlarmRingtone", AlarmRingtone);
                await repo.SetValue("MainFont", MainFont != null ? FontToString(MainFont) : null);
            }
            catch (RemoteException ex)
            {
                ExceptionForm.ShowConnectingError(ex);
            }
        }

        public static void ActualizeStartWithWindows()
        {
            const string regKey = "SlepoffStore";

            using var reg = Microsoft.Win32.Registry.CurrentUser
                .CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

            var regValue = reg.GetValue(regKey) as string;

            if (StartWithWindows)
            {
                var autorunCommandLine =
                    $"{Application.ExecutablePath} /server: \"{Program.ServerUrl}\" /username: \"{Program.UserName}\" /password: \"{Program.Password}\"";
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
