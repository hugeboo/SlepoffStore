using SlepoffStore.Core;
using SlepoffStore.Repository;
using SlepoffStore.Tools;

namespace SlepoffStore
{
    public partial class MainForm : Form
    {
        private const int WM_QUERYENDSESSION = 0x11;
        private bool _systemShutdown;
        private SheetsManager _sheetsManager;
        private InitMode _initMode;

        public enum InitMode
        {
            Normal,
            Settings
        }

        public bool ForceClose { get; set; }

        public MainForm()
        {
            InitializeComponent();
            sectionsTreeViewControl.CategorySelected += SectionsTreeViewControl_CategorySelected;
            sectionsTreeViewControl.SectionSelected += SectionsTreeViewControl_SectionSelected;
        }

        public MainForm Init(SheetsManager sm, InitMode initMode)
        {
            _sheetsManager = sm;
            _sheetsManager.SheetsListChanged += sheetsManager_SheetsListChanged;
            _initMode = initMode;
            return this;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_QUERYENDSESSION)
            {
                _systemShutdown = true;
            }
            base.WndProc(ref m);
        }

        private async void sheetsManager_SheetsListChanged(object? sender, GenericEventArgs<SheetForm[]> e)
        {
            try
            {
                using var repo = Program.CreateRepository();
                var uiSheets = await repo.ReadUISheets();
                var ds = dataGridView.DataSource as EntryGridItem[];
                foreach (var item in ds)
                {
                    item.Displayed = uiSheets.Any(it => it.EntryId == item.Id);
                }
                dataGridView.Invalidate();
            }
            catch (RemoteException ex)
            {
                ExceptionForm.ShowConnectingError(ex);
            }
        }

        private async void SectionsTreeViewControl_SectionSelected(object? sender, GenericEventArgs<SectionEx> e)
        {
            try
            {
                using var repo = Program.CreateRepository();
                var uiSheets = await repo.ReadUISheets();
                var items = (await repo.ReadEntriesBySectionId(e.Data.Id))
                    .Select(e => new EntryGridItem(e) { Displayed = uiSheets.Any(it => it.EntryId == e.Id) })
                    .ToArray();
                dataGridView.DataSource = items;
            }
            catch (RemoteException ex)
            {
                ExceptionForm.ShowConnectingError(ex);
            }
        }

        private async void SectionsTreeViewControl_CategorySelected(object? sender, GenericEventArgs<Category> e)
        {
            try
            {
                using var repo = Program.CreateRepository();
                var uiSheets = await repo.ReadUISheets();
                var items = (await repo.ReadEntriesByCategoryId(e.Data.Id))
                    .Select(e => new EntryGridItem(e) { Displayed = uiSheets.Any(it => it.EntryId == e.Id) })
                    .ToArray();
                dataGridView.DataSource = items;
            }
            catch (RemoteException ex)
            {
                ExceptionForm.ShowConnectingError(ex);
            }
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await sectionsTreeViewControl.Init();
            await _sheetsManager.RestoreAllSheets();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (_initMode == InitMode.Settings)
            {
                settingsToolStripButton_Click(this, EventArgs.Empty);
            }
        }

        private async void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Displayed
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView.Columns.IndexOf(displayedDataGridViewTextBoxColumn))
            {
                var item = dataGridView.Rows[e.RowIndex].DataBoundItem as EntryGridItem;
                if (item != null)
                {
                    if (item.Displayed)
                    {
                        await _sheetsManager.CloseSheet(item.Entry);
                        item.Displayed = false;
                    }
                    else
                    {
                        await _sheetsManager.ShowSheet(item.Entry);
                        item.Displayed = true;
                    }
                }
            }
        }

        private async void refreshToolStripButton_Click(object sender, EventArgs e)
        {
            await sectionsTreeViewControl.Init();
        }

        private async void settingsToolStripButton_Click(object sender, EventArgs e)
        {
            var form = new SettingsForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                Settings.StartWithWindows = form.StartWithWindows;
                Settings.AlarmRingtone = form.AlarmRingtone;
                Settings.MainFont = form.MainFont;
                await Settings.Save();
                Settings.ActualizeStartWithWindows();
                _sheetsManager.RefreshAllSheets();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ForceClose && !_systemShutdown)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private async void addToolStripButton_Click(object sender, EventArgs e)
        {
            await _sheetsManager.AddNew();
            await sectionsTreeViewControl.Init();
        }

        private async void deleteToolStripButton_Click(object sender, EventArgs e)
        {
            var count = dataGridView.SelectedRows.Count;
            if (count < 1) return;
            var s = count == 1 ? "entry" : "entries";

            if (MessageBox.Show(this,
                $"Are you sure you want to delete the selected {count} {s}?", "Delete entries",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var lst = new List<Entry>();
                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                {
                    var item = row.DataBoundItem as EntryGridItem;
                    if (item?.Entry != null) lst.Add(item.Entry);
                }
                await _sheetsManager.DeleteEntries(lst.ToArray());
                await sectionsTreeViewControl.Init();
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            deleteToolStripButton.Enabled = dataGridView.SelectedRows.Count > 0;
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