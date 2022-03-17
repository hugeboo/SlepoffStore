using SlepoffStore.Core;
using SlepoffStore.Tools;

namespace SlepoffStore
{
    internal static class Program
    {
        private static MainForm _mainForm;

        public enum SourceType
        {
            SQLiteFile,
            WebService
        }

        public static SourceType Source { get;private set; }
        public static string SourceUrl { get; private set; }
        public static string UserName { get; private set; }

        [STAThread]
        static void Main()
        {
            //// To customize application configuration such as set high DPI settings or default font,
            //// see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var cla = new CommandLine();
            if (!cla.IsOK) return;

            Source = cla.Source.Value;
            SourceUrl = cla.SourceUrl;
            UserName = cla.UserName;

            Settings.Load();
            Settings.ActualizeStartWithWindows();

            var sm = new SheetsManager();
            sm.RestoreAllSheets();

            using (NotifyIcon icon = new NotifyIcon())
            {
                icon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                var menu = new ContextMenuStrip();
                menu.Items.AddRange(new ToolStripItem[]
                {
                    new ToolStripMenuItem("Add New", null, (s, e) => sm.AddNew()),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Open Store Window...", null, (s, e) => OpenMainForm(sm, MainForm.InitMode.Normal)),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Restore All", null, (s, e) => sm.RestoreAllSheets()),
                    new ToolStripMenuItem("Collapse All", null, (s, e) => sm.CollapseAllSheets()),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Settings...", null, (s, e) => OpenMainForm(sm, MainForm.InitMode.Settings)),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Exit", null, (s, e) => Application.Exit()),
                });
                icon.ContextMenuStrip = menu;
                icon.Visible = true;

                Application.Run();
                icon.Visible = false;
            }
        }

        public static IRepository CreateRepository()
        {
            if (Source == SourceType.SQLiteFile)
            {
                return new SQLiteRepository(SourceUrl, UserName, Environment.MachineName);
            }
            else if (Source == SourceType.WebService)
            {
                return new RemoteRepository(SourceUrl, UserName, Environment.MachineName);
            }
            else
            {
                throw new Exception("Unknown SourceType");
            }
        }

        private static void OpenMainForm(SheetsManager sm, MainForm.InitMode mode)
        {
            if (_mainForm == null || _mainForm.IsDisposed)
            {
                _mainForm = new MainForm().Init(sm, mode);
                _mainForm.Show();
                _mainForm.BringToFront();
            }
            else
            {
                _mainForm.Activate();
            }
        }

        internal sealed class CommandLine
        {
            private const string ErrorMessage1 = 
                "Database is unspecified!\n\n" +
                "Use command line:\n\n" +
                "slepoffstore /database: <filename> /username: <username>\n" +
                "or\n" +
                "slepoffstore /server: <url> /username: <username>";

            private const string ErrorMessage2 =
                "UserName is unspecified!\n\n" +
                "Use command line:\n\n" +
                "slepoffstore.exe /database: <filename> /username: <username>\n" +
                "or\n" +
                "slepoffstore.exe /server: <url> /username: <username>";

            public SourceType? Source { get; private set; }
            public string SourceUrl { get; private set; }
            public string UserName { get; private set; }

            public bool IsOK { get; private set; }

            public CommandLine()
            {
                IsOK = Parse();
            }

            private bool Parse()
            {
                var args = Environment.GetCommandLineArgs();
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].ToLower() == "/database:" && i < args.Length - 1)
                    {
                        if (Source.HasValue && Source == SourceType.WebService)
                        {
                            MessageBox.Show(ErrorMessage1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return false;
                        }
                        Source = SourceType.SQLiteFile;
                        SourceUrl = args[++i];
                    }
                    else if (args[i].ToLower() == "/server:" && i < args.Length - 1)
                    {
                        if (Source.HasValue && Source == SourceType.SQLiteFile)
                        {
                            MessageBox.Show(ErrorMessage1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return false;
                        }
                        Source = SourceType.WebService;
                        SourceUrl = args[++i];
                    }
                    else if (args[i].ToLower() == "/username:" && i < args.Length - 1)
                    {
                        UserName = args[++i];
                    }
                }

                if (!Source.HasValue || string.IsNullOrWhiteSpace(SourceUrl))
                {
                    MessageBox.Show(ErrorMessage1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(UserName))
                {
                    MessageBox.Show(ErrorMessage2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

                return true;
            }
        }
    }
}