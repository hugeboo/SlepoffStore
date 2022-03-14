using SlepoffStore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore
{
    public static class Settings
    {
        public static bool StartWithWindows { get; set; }

        public static void Load()
        {
            using var repo = new Repository();
            StartWithWindows = repo["StartWithWindows"] == "true";
        }

        public static void Save()
        {
            using var repo = new Repository();
            repo["StartWithWindows"] = StartWithWindows.ToString().ToLower();
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
    }
}
