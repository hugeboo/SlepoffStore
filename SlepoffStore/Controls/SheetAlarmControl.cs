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
        public SheetAlarmControl()
        {
            InitializeComponent();
        }

        public void Init(Entry entry)
        {
            if (entry.Alarm.HasValue && entry.AlarmIsOn)
            {
                imageLabel.Image = entry.Color.GetForeColor() == Color.White ?
                    Properties.Resources.icons8_будильник_24_white : Properties.Resources.icons8_будильник_24_black;

                textLabel.ForeColor = entry.Color.GetForeColor();
                textLabel.Text = entry.Alarm.Value.ToString("f");

                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
        }
    }
}
