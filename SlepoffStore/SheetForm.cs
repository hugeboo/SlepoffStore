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
using Templates;

namespace SlepoffStore
{
    public partial class SheetForm : BorderLessForm
    {
        private EntryColor _currentColor;
        private bool _prevAlarmActivted;

        public UISheet UISheet { get; private set; }
        public Entry Entry { get; private set; }

        public Font MainFont
        {
            get => textBox.Font;
            set
            {
                if (value!=null) textBox.Font = value; ;
            }
        }

        public bool AlarmActivated => sheetAlarmControl.AlarmActivated;

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
            _currentColor = Entry.Color;
            RestoreColors();
            sheetAlarmControl.Init(Entry);
            this.Invalidate();
        }

        private void RestoreColors()
        {
            this.BackColor = _currentColor.ToColor();
            textBox.BackColor = _currentColor.ToColor();
            textBox.ForeColor = _currentColor.GetForeColor();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Entry != null)
            {
                using var brush = new SolidBrush(_currentColor.GetForeColor());
                var sf = new StringFormat(StringFormatFlags.NoWrap) 
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                e.Graphics.DrawString(Entry.Caption, this.Font, brush, 
                    new Rectangle(0, 0, this.Width, HeaderHeight), sf);
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            captionToolStripTextBox.Text = Entry?.Caption;
            colorsToolStripComboBox.SelectedItem = Entry?.Color.ToString();
        }

        private void colorsToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Entry == null) return;
            if (Entry.Color.ToString() != colorsToolStripComboBox.Text)
            {
                Entry.Color = EntryColor.Parse<EntryColor>(colorsToolStripComboBox.Text);
                using var repo = new Repository();
                repo.UpdateEntry(Entry);
                _currentColor = Entry.Color;
                RestoreColors();
                sheetAlarmControl.Init(Entry);
                this.Invalidate();
            }
        }

        private void captionToolStripTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                contextMenuStrip.Close();
            }
        }

        private void contextMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (Entry == null) return;
            if (Entry.Caption != captionToolStripTextBox.Text)
            {
                Entry.Caption = captionToolStripTextBox.Text;
                using var repo = new Repository();
                repo.UpdateEntry(Entry);
                this.Invalidate();
            }
        }

        private void setAlarmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AlarmEditorForm();
            if (!Entry.Alarm.HasValue)
            {
                // first time alarm is on by default
                form.AlarmEnabled = true;
            }
            else
            {
                form.AlarmDateTime = Entry.Alarm.Value;
                form.AlarmEnabled = Entry.AlarmIsOn;
            }
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                using var repo = new Repository();
                Entry.Alarm = form.AlarmDateTime;
                Entry.AlarmIsOn = form.AlarmEnabled;
                repo.UpdateEntry(Entry);
                sheetAlarmControl.Init(Entry);
            }
        }

        private void flashTimer_Tick(object sender, EventArgs e)
        {
            if (AlarmActivated)
            {
                _currentColor = _currentColor == EntryColor.White ? EntryColor.Black : EntryColor.White;
                RestoreColors();
                this.Invalidate();
            }
            else
            {
                if (_prevAlarmActivted)
                {
                    _currentColor = Entry.Color;
                    RestoreColors();
                    this.Invalidate();
                }
            }
            _prevAlarmActivted = AlarmActivated;
        }
    }

    public sealed class UISheetChangedEventArgs : EventArgs
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
    }
}
