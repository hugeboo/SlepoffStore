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

namespace SlepoffStore.Controls
{
    public partial class SheetAlarmControl : UserControl
    {
        private Entry _entry;
        private bool _isWhiteAlarmImage;

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
                var backColor = entry.Color.ToColor().AdjustLightnessAndSaturation(1.2, 0.8);
                imageLabel.BackColor = backColor;
                if ( entry.Color.GetForeColor() == Color.White) SetWhiteAlarmImage(); else SetBlackAlarmImage();

                textLabel.BackColor = backColor;
                textLabel.ForeColor = entry.Color.GetForeColor();
                textLabel.Text = entry.Alarm.Value.ToString("f");

                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
        }

        public void ToggleAlarmImage()
        {
            if (_isWhiteAlarmImage) SetBlackAlarmImage(); else SetWhiteAlarmImage();
        }

        private void SetWhiteAlarmImage()
        {
            imageLabel.Image = Properties.Resources.icons8_будильник_24_white;
            _isWhiteAlarmImage = true;
        }

        private void SetBlackAlarmImage()
        {
            imageLabel.Image = Properties.Resources.icons8_будильник_24_black;
            _isWhiteAlarmImage = false;
        }

        private async Task DisableAlarm(MouseEventArgs e)
        {
            try
            {
                if (AlarmActivated && e.Button == MouseButtons.Left)
                {
                    using var repo = Program.CreateRepository();
                    _entry.AlarmIsOn = false;
                    await repo.UpdateEntry(_entry);
                    this.Visible = false;
                }
            }
            catch (RemoteException ex)
            {
                ExceptionForm.ShowConnectingError(ex);
            }
        }

        private async void textLabel_MouseDown(object sender, MouseEventArgs e)
        {
            await DisableAlarm(e);
        }

        private async void imageLabel_MouseDown(object sender, MouseEventArgs e)
        {
            await DisableAlarm(e);
        }
    }
}
