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

namespace SlepoffStore.Controls
{
    public partial class SheetAlarmControl : UserControl
    {
        private Entry _entry;

        public bool AlarmActivated => _entry != null &&
            _entry.AlarmIsOn && _entry.Alarm.HasValue && _entry.Alarm.Value < DateTime.Now;

        public SheetAlarmControl()
        {
            InitializeComponent();
        }

        public void Init(Entry entry)
        {
            _entry = entry;
            if (entry.Alarm.HasValue && entry.AlarmIsOn)
            {
                imageLabel.BackColor = entry.Color.ToColor();
                imageLabel.Image = entry.Color.GetForeColor() == Color.White ?
                    Properties.Resources.icons8_будильник_24_white : Properties.Resources.icons8_будильник_24_black;

                textLabel.BackColor = entry.Color.ToColor();
                textLabel.ForeColor = entry.Color.GetForeColor();
                textLabel.Text = entry.Alarm.Value.ToString("f");

                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
        }

        private void DisableAlarm(MouseEventArgs e)
        {
            if (AlarmActivated && e.Button == MouseButtons.Left)
            {
                using var repo = new Repository();
                _entry.AlarmIsOn = false;
                repo.UpdateEntry(_entry);
                this.Visible = false;
            }
        }

        private void textLabel_MouseDown(object sender, MouseEventArgs e)
        {
            DisableAlarm(e);
        }

        private void imageLabel_MouseDown(object sender, MouseEventArgs e)
        {
            DisableAlarm(e);
        }
    }
}
