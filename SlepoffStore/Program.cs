using SlepoffStore.Model;
using SlepoffStore.Tools;

namespace SlepoffStore
{
    internal static class Program
    {
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
            Repository.DatabaseName = cla.DatabaseName;

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
                    new ToolStripMenuItem("Open Store...", null, (s, e) => new MainForm().Init(sm).Show()),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Restore All", null, (s, e) => sm.RestoreAllSheets()),
                    new ToolStripMenuItem("Collapse All", null, (s, e) => sm.CollapseAllSheets()),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Exit", null, (s, e) => Application.Exit()),
                });
                icon.ContextMenuStrip = menu;
                icon.Visible = true;

                Application.Run();
                icon.Visible = false;
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