using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SlepoffStore
{
    public partial class LogonForm : Form
    {
        public string UserName
        {
            get => userTextBox.Text;
            set => userTextBox.Text = value;
        }

        public string Password
        {
            get => passwordTextBox.Text;
            set => passwordTextBox.Text = value;
        }   

        public bool PasswordEnabled
        {
            get => passwordTextBox.Enabled;
            set => passwordTextBox.Enabled = value;
        }

        public LogonForm()
        {
            InitializeComponent();
        }

        private void LogonForm_Load(object sender, EventArgs e)
        {
            this.Text = $"Slepoff Store - {Program.ServerUrl}";
        }
    }
}
