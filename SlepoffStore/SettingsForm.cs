using SlepoffStore.Model;
using SlepoffStore.Tools;
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
        private Font _mainFont;

        public bool StartWithWindows
        {
            get => startCheckBox.Checked;
            set => startCheckBox.Checked = value;
        }

        public Font MainFont
        {
            get { return _mainFont; }
            set 
            { 
                _mainFont = value;
                fontTextBox.Text = Settings.FontToString(value);
            }
        }

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void fontButton_Click(object sender, EventArgs e)
        {
            fontDialog.Font = MainFont;
            if (fontDialog.ShowDialog(this) == DialogResult.OK)
            {
                MainFont = fontDialog.Font;
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            StartWithWindows = Settings.StartWithWindows;
            MainFont = Settings.MainFont;
        }
    }
}
