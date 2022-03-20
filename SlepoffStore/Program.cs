using SlepoffStore.Core;
using SlepoffStore.Tools;

namespace SlepoffStore
{
    internal static class Program
    {
        public enum SourceType
        {
            SQLiteFile,
            WebService
        }

        public static SourceType Source { get;private set; }
        public static string SourceUrl { get; private set; }
        public static string UserName { get; private set; }
        public static string Password { get; private set; }

        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            //// To customize application configuration such as set high DPI settings or default font,
            //// see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var cla = new CommandLine();
            if (!cla.IsOK) return;

            Source = cla.Source.Value;
            SourceUrl = cla.SourceUrl;
            UserName = cla.UserName;
            Password = cla.Password;

            if (!RequestAuthInfo()) return;

            Settings.Load();
            Settings.ActualizeStartWithWindows();

            var sm = new SheetsManager();

            var mainForm = new MainForm().Init(sm, MainForm.InitMode.Normal);
            mainForm.ShowInTaskbar = false;
            mainForm.WindowState = FormWindowState.Minimized;
            mainForm.Show();

            using var icon = new NotifyIcon();
            icon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            var menu = new ContextMenuStrip();
            menu.Items.AddRange(new ToolStripItem[]
            {
                    new ToolStripMenuItem("Add New", null, async (s, e) => await sm.AddNew()),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Open Store Window...", null, (s, e) =>
                    {
                        mainForm.WindowState = FormWindowState.Normal;
                        mainForm.Activate();
                    }),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Restore All", null, async (s, e) => await sm.RestoreAllSheets()),
                    new ToolStripMenuItem("Collapse All", null, (s, e) => sm.CollapseAllSheets()),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Exit", null, (s, e) => 
                    {
                        mainForm.ForceClose = true;
                        mainForm.Close(); 
                    })
            });
            icon.ContextMenuStrip = menu;
            icon.Visible = true;

            Application.Run(mainForm);
            icon.Visible = false;
        }

        public static IRepository CreateRepository()
        {
            if (Source == SourceType.SQLiteFile)
            {
                return new SQLiteRepository(SourceUrl, UserName, Environment.MachineName);
            }
            else if (Source == SourceType.WebService)
            {
                return new RemoteRepository(SourceUrl, UserName, Password, Environment.MachineName);
            }
            else
            {
                throw new Exception("Unknown SourceType");
            }
        }

        private static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = args.ExceptionObject as Exception;
            MessageBox.Show(e?.Message ?? "Unknown error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            Environment.Exit(1);
        }

        private static bool RequestAuthInfo()
        {
            if (Source == SourceType.SQLiteFile)
            {
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    var form = new LogonForm
                    {
                        UserName = null,
                        Password = null,
                        PasswordEnabled = false
                    };
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        UserName = form.UserName;
                        return !string.IsNullOrWhiteSpace(UserName);
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (Source == SourceType.WebService)
            {
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
                {
                    var form = new LogonForm
                    {
                        UserName = UserName,
                        Password = Password,
                        PasswordEnabled = true
                    };
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        UserName = form.UserName;
                        Password = form.Password;
                        return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        internal sealed class CommandLine
        {
            private const string ErrorMessage1 =
                "Database is unspecified!\n\n" +
                "Use command line:\n\n" +
                "slepoffstore /database: <filename>\n" +
                "or\n" +
                "slepoffstore /server: <url>";

            public SourceType? Source { get; private set; }
            public string SourceUrl { get; private set; }
            public string UserName { get; private set; }
            public string Password { get; private set; }

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
                    else if (args[i].ToLower() == "/password:" && i < args.Length - 1)
                    {
                        Password = args[++i];
                    }
                }

                if (!Source.HasValue || string.IsNullOrWhiteSpace(SourceUrl))
                {
                    MessageBox.Show(ErrorMessage1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

                return true;
            }
        }
    }
}