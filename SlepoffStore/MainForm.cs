using SlepoffStore.Model;

namespace SlepoffStore
{
    public partial class MainForm : Form
    {
        private SheetsManager _sheetsManager;

        public MainForm()
        {
            InitializeComponent();
            sectionsTreeViewControl.CategorySelected += SectionsTreeViewControl_CategorySelected;
            sectionsTreeViewControl.SectionSelected += SectionsTreeViewControl_SectionSelected;
        }

        public MainForm Init(SheetsManager sm)
        {
            _sheetsManager = sm;
            return this;
        }

        private void SectionsTreeViewControl_SectionSelected(object? sender, GenericEventArgs<SectionEx> e)
        {
            using var repo = new Repository();
            var uiSheets = repo.GetUISheets();
            var items = repo.GetEntriesBySectionId(e.Data.Id)
                .Select(e => new EntryGridItem(e) { Displayed = uiSheets.Any(it => it.EntryId == e.Id) })
                .ToArray();
            dataGridView.DataSource = items;
        }

        private void SectionsTreeViewControl_CategorySelected(object? sender, GenericEventArgs<Category> e)
        {
            using var repo = new Repository();
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

        private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Displayed
            if (e.RowIndex >= 0 && e.ColumnIndex == 1)
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
    }

    public sealed class EntryGridItem
    {
        public Entry Entry { get; }

        public long Id => Entry.Id;
        public string Caption => Entry.Caption;
        public DateTime CreationDate => Entry.CreationDate;
        public string Text => Entry.Text;
        public bool Displayed { get; set; }

        public EntryGridItem(Entry entry)
        {
            Entry = entry;
        }
    }
}