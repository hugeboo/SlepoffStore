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
    public partial class AlarmEditorForm : Form
    {
        public bool AlarmEnabled
        {
            get => enabledCheckBox.Checked;
            set => enabledCheckBox.Checked = value;
        }

        public DateTime AlarmDateTime
        {
            get
            {
                var d = datePicker.Value;
                var t = timePicker.Value;
                return new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, 0);
            }
            set
            {
                datePicker.Value = value;
                timePicker.Value = value;
            }
        }


        public AlarmEditorForm()
        {
            InitializeComponent();
        }
    }
}
