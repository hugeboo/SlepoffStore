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
    public partial class SheetForm : Form
    {
        public UISheet UISheet { get; private set; }
        public Entry Entry { get; private set; }

        public SheetForm()
        {
            InitializeComponent();
        }

        public void Init(Entry entry, UISheet uiSheet)
        {
            Entry = entry;
            UISheet = uiSheet;
            this.Text = Entry.Caption;
            textBox.Text = Entry.Text;
        }

        private void OnTitlebarClick(Point pos)
        {
            contextMenuStrip.Show(pos);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCRBUTTONDOWN = 0xa4;
            if (m.Msg == WM_NCRBUTTONDOWN)
            {
                var pos = new Point(m.LParam.ToInt32());
                OnTitlebarClick(pos);
                return;
            }
            base.WndProc(ref m);
        }

        private void SheetForm_Activated(object sender, EventArgs e)
        {
            this.SendToBack();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Entry != null && UISheet != null &&
                (textBox.Text != Entry.Text ||
                this.Location.X != UISheet.PosX || this.Location.Y != UISheet.PosY ||
                this.Width != UISheet.Width || this.Height != UISheet.Height))
            {
                using var repo = new Repository();
                if (textBox.Text != Entry.Text)
                {
                    Entry.Text = textBox.Text;
                    repo.UpdateEntry(Entry);
                }
                if (this.Location.X != UISheet.PosX || this.Location.Y != UISheet.PosY ||
                    this.Width != UISheet.Width || this.Height != UISheet.Height)
                {
                    UISheet.PosX = this.Location.X;
                    UISheet.PosY = this.Location.Y;
                    UISheet.Width = this.Width;
                    UISheet.Height = this.Height;
                    repo.UpdateUISheet(UISheet);
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "The note will be hidden, but the entry will remain in the database. Proceed?", "Slepoff Store", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using var repo = new Repository();
                repo.DeleteUISheet(UISheet);
                this.Close();
            }
        }
    }

    public sealed class UISheetChangedEventArgs : EventArgs
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
    }
}
