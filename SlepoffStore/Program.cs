using SlepoffStore.Core;
using SlepoffStore.Repository;
using SlepoffStore.Tools;

namespace SlepoffStore
{
    internal static class Program
    {
        public static string ServerUrl { get; private set; }
        public static string UserName { get; private set; }
        public static string Password { get; private set; }

        [STAThread]
        static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionEventHandler);

            //// To customize application configuration such as set high DPI settings or default font,
            //// see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var cla = new CommandLine();
            if (!cla.IsOK) return;

            ServerUrl = cla.ServerUrl;
            UserName = cla.UserName;
            Password = cla.Password;

            if (!RequestAuthInfo()) return;

            await Settings.Load();
            Settings.ActualizeStartWithWindows();

            var sm = new SheetsManager();

            var mainForm = new MainForm().Init(sm, MainForm.InitMode.Normal);
            mainForm.ShowInTaskbar = false;
            mainForm.WindowState = FormWindowState.Minimized;
            mainForm.Show();

            using var icon = new NotifyIcon();
            icon.Text = "Slepoff Store";
            icon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            var menu = new ContextMenuStrip();
            menu.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Add New", null, async (s, e) => await sm.AddNew()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Open Store Window...", null, (s, e) =>
                {
                    mainForm.WindowState = FormWindowState.Normal;
                    mainForm.Show();
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
            return new RemoteRepository(ServerUrl, UserName, Password, Environment.MachineName);
        }

        private static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = args.ExceptionObject as Exception;
            if (e == null)
            {
                MessageBox.Show("Unknown error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                new ExceptionForm().Init(ExceptionForm.UNKNOWN_ERROR, e);
            }
            Environment.Exit(1);
        }

        private static bool RequestAuthInfo()
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

        internal sealed class CommandLine
        {
            private const string ErrorMessage1 =
                "Database is unspecified!\n\n" +
                "Use command line: slepoffstore /server: <url>";

            public string ServerUrl { get; private set; }
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
                    if (args[i].ToLower() == "/server:" && i < args.Length - 1)
                    {
                        ServerUrl = args[++i];
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

                if (string.IsNullOrWhiteSpace(ServerUrl))
                {
                    MessageBox.Show(ErrorMessage1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

                return true;
            }
        }
    }
}