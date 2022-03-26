using SlepoffStore.Core;
using SlepoffStore.Repository;
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
using Templates;

namespace SlepoffStore
{
    public partial class SheetForm : BorderLessForm
    {
        private EntryColor _currentColor;
        private bool _prevAlarmActivted;
        private static bool _timerTickExceptionVisible;
        private bool _timerTickDataSaved = true;

        public UISheet UISheet { get; private set; }
        public Entry Entry { get; private set; }

        public Font MainFont
        {
            get => textBox.Font;
            set
            {
                if (value != null) textBox.Font = value; ;
            }
        }

        public bool AlarmActivated => sheetAlarmControl.AlarmActivated;

        public bool WaitMode
        {
            get => loadingPictureBox.Visible;
            set => loadingPictureBox.Visible = value;
        }

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
            this.BackColor = _currentColor.ToColor().AdjustLightnessAndSaturation(1.2, 0.8);
            if (AlarmActivated)
            {
                this.HeaderBackColor = Color.Transparent;
            }
            else
            {
                this.HeaderBackColor = this.BackColor.AdjustLightness(_currentColor.IsDark() ? 1.2 : 0.8);
            }

            textBox.BackColor = this.BackColor;
            textBox.ForeColor = _currentColor.GetForeColor();
        }

        private void SheetForm_Activated(object sender, EventArgs e)
        {
            // без этого вроде лучше...
            //if (!AlarmActivated) this.SendToBack();
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            if (Entry != null && UISheet != null &&
                (!_timerTickDataSaved ||
                textBox.Text != Entry.Text ||
                this.Location.X != UISheet.PosX || this.Location.Y != UISheet.PosY ||
                this.Width != UISheet.Width || this.Height != UISheet.Height))
            {
                WaitMode = true;
                try
                {
                    using var repo = Program.CreateRepository();
                    if (!_timerTickDataSaved || 
                        textBox.Text != Entry.Text)
                    {
                        Entry.Text = textBox.Text;
                        await repo.UpdateEntry(Entry);
                    }
                    if (!_timerTickDataSaved || 
                        this.Location.X != UISheet.PosX || this.Location.Y != UISheet.PosY ||
                        this.Width != UISheet.Width || this.Height != UISheet.Height)
                    {
                        UISheet.PosX = this.Location.X;
                        UISheet.PosY = this.Location.Y;
                        UISheet.Width = this.Width;
                        UISheet.Height = this.Height;
                        await repo.UpdateUISheet(UISheet);
                    }
                    _timerTickDataSaved = true;
                    this.Opacity = 1.0;
                }
                catch (RemoteException ex)
                {
                    if (!_timerTickExceptionVisible && _timerTickDataSaved)
                    {
                        _timerTickExceptionVisible = true;
                        ExceptionForm.ShowConnectingError(ex, () => _timerTickExceptionVisible = false);
                    }
                    _timerTickDataSaved = false;
                    this.Opacity = 0.3;
                }
                finally
                {
                    WaitMode = false;
                }
            }
        }

        private async void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "The note will be hidden, but the entry will remain in the database. Proceed?", "Slepoff Store",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                WaitMode = true;
                try
                {
                    using var repo = Program.CreateRepository();
                    await repo.DeleteUISheet(UISheet);
                    this.Close();
                }
                catch (RemoteException ex)
                {
                    ExceptionForm.ShowConnectingError(ex);
                }
                finally
                {
                    WaitMode = false;
                }
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
                    new Rectangle(10 + 22, 0, this.Width - (10 + 22) * 2, HeaderHeight), sf);
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            captionToolStripTextBox.Text = Entry?.Caption;
            colorsToolStripComboBox.SelectedItem = Entry?.Color.ToString();
        }

        private async void colorsToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Entry == null) return;
            if (Entry.Color.ToString() != colorsToolStripComboBox.Text)
            {
                var oldColor = Entry.Color;
                WaitMode = true;
                try
                {
                    Entry.Color = EntryColor.Parse<EntryColor>(colorsToolStripComboBox.Text);
                    using var repo = Program.CreateRepository();
                    await repo.UpdateEntry(Entry);
                    _currentColor = Entry.Color;
                    RestoreColors();
                    sheetAlarmControl.Init(Entry);
                    this.Invalidate();
                }
                catch (RemoteException ex)
                {
                    Entry.Color = oldColor;
                    ExceptionForm.ShowConnectingError(ex);
                }
                finally
                {
                    WaitMode = false;
                }
            }
        }

        private void captionToolStripTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                contextMenuStrip.Close();
            }
        }

        private async void contextMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (Entry == null) return;
            if (Entry.Caption != captionToolStripTextBox.Text)
            {
                var oldCaption = Entry.Caption;
                WaitMode = true;
                try
                {
                    Entry.Caption = captionToolStripTextBox.Text;
                    using var repo = Program.CreateRepository();
                    await repo.UpdateEntry(Entry);
                    this.Invalidate();
                }
                catch (RemoteException ex)
                {
                    Entry.Caption = oldCaption;
                    ExceptionForm.ShowConnectingError(ex);
                }
                finally
                {
                    WaitMode = false;
                }
            }
        }

        private async void setAlarmToolStripMenuItem_Click(object sender, EventArgs e)
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
                var oldAlarm = Entry.Alarm;
                var oldAlarmIsOn = Entry.AlarmIsOn;
                WaitMode = true;
                try
                {
                    using var repo = Program.CreateRepository();
                    Entry.Alarm = form.AlarmDateTime;
                    Entry.AlarmIsOn = form.AlarmEnabled;
                    await repo.UpdateEntry(Entry);
                    sheetAlarmControl.Init(Entry);
                }
                catch (RemoteException ex)
                {
                    Entry.Alarm = oldAlarm;
                    Entry.AlarmIsOn = oldAlarmIsOn;
                    ExceptionForm.ShowConnectingError(ex);
                }
                finally
                {
                    WaitMode = false;
                }
            }
        }

        private void flashTimer_Tick(object sender, EventArgs e)
        {
            if (AlarmActivated)
            {
                if (!_prevAlarmActivted)
                {
                    this.Activate();
                    this.BringToFront();
                }
                flashTimer.Interval = 500;
                _currentColor = _currentColor == EntryColor.White ? EntryColor.Black : EntryColor.White;
                RestoreColors();
                this.Invalidate();
            }
            else
            {
                if (_prevAlarmActivted)
                {
                    this.SendToBack();
                    flashTimer.Interval = 2000;
                    _currentColor = Entry.Color;
                    RestoreColors();
                    this.Invalidate();
                }
            }
            _prevAlarmActivted = AlarmActivated;
        }

        private void SheetForm_Load(object sender, EventArgs e)
        {
            this.SendToBack();
        }
    }

    public sealed class UISheetChangedEventArgs : EventArgs
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
    }
}
