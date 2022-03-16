using SlepoffStore.Core;
using SlepoffStore.Tools;

namespace SlepoffStore
{
    public partial class MainForm : Form
    {
        private SheetsManager _sheetsManager;
        private InitMode _initMode;

        public enum InitMode
        {
            Normal,
            Settings
        }

        public MainForm()
        {
            InitializeComponent();
            sectionsTreeViewControl.CategorySelected += SectionsTreeViewControl_CategorySelected;
            sectionsTreeViewControl.SectionSelected += SectionsTreeViewControl_SectionSelected;
        }

        public MainForm Init(SheetsManager sm, InitMode initMode)
        {
            _sheetsManager = sm;
            _initMode = initMode;
            return this;
        }

        private void SectionsTreeViewControl_SectionSelected(object? sender, GenericEventArgs<SectionEx> e)
        {
            using var repo = Program.CreateRepository();
            var uiSheets = repo.GetUISheets();
            var items = repo.GetEntriesBySectionId(e.Data.Id)
                .Select(e => new EntryGridItem(e) { Displayed = uiSheets.Any(it => it.EntryId == e.Id) })
                .ToArray();
            dataGridView.DataSource = items;
        }

        private void SectionsTreeViewControl_CategorySelected(object? sender, GenericEventArgs<Category> e)
        {
            using var repo = Program.CreateRepository();
            var uiSheets = repo.GetUISheets();
            var items = repo.GetEntriesByCategoryId(e.Data.Id)
                .Select(e => new EntryGridItem(e) { Displayed = uiSheets.Any(it => it.EntryId == e.Id) })
                .ToArray();
            dataGridView.DataSource = items;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            sectionsTreeViewControl.Init();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (_initMode == InitMode.Settings)
            {
                settingsToolStripButton_Click(this, EventArgs.Empty);
            }
        }

        private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Displayed
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView.Columns.IndexOf(displayedDataGridViewTextBoxColumn))
            {
                var item = dataGridView.Rows[e.RowIndex].DataBoundItem as EntryGridItem;
                if (item != null)
                {
                    if (item.Displayed)
                    {
                        _sheetsManager.CloseSheet(item.Entry);
                        item.Displayed = false;
                    }
                    else
                    {
                        _sheetsManager.ShowSheet(item.Entry);
                        item.Displayed = true;
                    }
                }
            }
         }

        private void refreshToolStripButton_Click(object sender, EventArgs e)
        {
            sectionsTreeViewControl.Init();
        }

        private void settingsToolStripButton_Click(object sender, EventArgs e)
        {
            var form = new SettingsForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                Settings.StartWithWindows = form.StartWithWindows;
                Settings.AlarmRingtone = form.AlarmRingtone;
                Settings.MainFont = form.MainFont;
                Settings.Save();
                Settings.ActualizeStartWithWindows();
                _sheetsManager.RefreshAllSheets();
            }
        }
    }

    public sealed class EntryGridItem
    {
        public Entry Entry { get; }

        public long Id => Entry.Id;
        public string Caption => Entry.Caption;
        public DateTime CreationDate => Entry.CreationDate;
        public string Text => Entry.Text;
        public DateTime? Alarm => Entry.Alarm;
        public bool AlarmIsOn => Entry.AlarmIsOn;
        public bool Displayed { get; set; }

        public EntryGridItem(Entry entry)
        {
            Entry = entry;
        }
    }
}