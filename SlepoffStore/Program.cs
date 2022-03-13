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
    }
}