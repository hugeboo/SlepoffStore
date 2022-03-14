using SlepoffStore.Model;
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
    public partial class SettingsForm : Form
    {
        public bool StartWithWindows => startCheckBox.Checked;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void fontButton_Click(object sender, EventArgs e)
        {
            if (fontDialog.ShowDialog(this) == DialogResult.OK)
            {

            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            startCheckBox.Checked = Settings.StartWithWindows;
        }
    }
}
