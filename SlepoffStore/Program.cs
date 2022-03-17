using SlepoffStore.Core;
using SlepoffStore.Tools;

namespace SlepoffStore
{
    internal static class Program
    {
        private static MainForm _mainForm;

        public static string DatabaseName { get; private set; }

        [STAThread]
        static void Main()
        {
            //// To customize application configuration such as set high DPI settings or default font,
            //// see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var cla = new CommandLine();
            if (string.IsNullOrEmpty(cla.DatabaseName))
            {
                MessageBox.Show("Database is unspecified!\nUse command line: slepoffstore.exe /database: <db_name>",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            DatabaseName = cla.DatabaseName;

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
            return new SQLiteRepository(DatabaseName, "root", "LAPTOP-SSV");
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
            public string DatabaseName { get; set; }

            public CommandLine()
            {
                Parse();
            }

            private void Parse()
            {
                var args = Environment.GetCommandLineArgs();
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].ToLower() == "/database:" && i < args.Length - 1)
                    {
                        DatabaseName = args[++i];
                    }
                }
            }
        }
    }
}