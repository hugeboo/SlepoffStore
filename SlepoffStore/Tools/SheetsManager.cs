using SlepoffStore.Core;
using SlepoffStore.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Tools
{
    public sealed class SheetsManager : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const string SECTION_FOR_NEW = "Other";
        private const string CATEGORY_FOR_NEW = "New";

        private readonly ConcurrentDictionary<long, SheetForm> _sheets = new();
        private readonly AlarmManager _alarmManger;

        public event EventHandler<GenericEventArgs<SheetForm[]>> SheetsListChanged;

        public SheetsManager()
        {
            _alarmManger = new AlarmManager(this);
        }

        public bool IsAlarm => _sheets.Any(kvp => kvp.Value.AlarmActivated);

        public bool Collapsed { get; private set; }

        public void CollapseAllSheets()
        {
            foreach (var kvp in _sheets)
            {
                kvp.Value.FormClosed -= SheetForm_FormClosed;
                kvp.Value.Close();
            }
            _sheets.Clear();
            SheetsListChanged?.Invoke(this, new GenericEventArgs<SheetForm[]>(_sheets.Values.ToArray()));
            Collapsed = true;
        }

        public async Task RestoreAllSheets()
        {
            using var repo = Program.CreateRepository();
            var uiSheets = await repo.ReadUISheets();

            var focus = GetForegroundWindow();

            foreach (var uiSheet in uiSheets)
            {
                if (!_sheets.Any(s => s.Value.UISheet.Id == uiSheet.Id))
                {
                    var form = CreateSheetForm();
                    form.Size = new Size(uiSheet.Width, uiSheet.Height);
                    form.Location = new Point(uiSheet.PosX, uiSheet.PosY);
                    form.Show();
                    form.Init(await repo.ReadEntry(uiSheet.EntryId), uiSheet);
                    _sheets[form.UISheet.Id] = form;
                }
            }

            SetForegroundWindow(focus);
            Collapsed = false;
            SheetsListChanged?.Invoke(this, new GenericEventArgs<SheetForm[]>(_sheets.Values.ToArray()));
        }

        public async Task DeleteEntries(Entry[] entries)
        {
            var repo = Program.CreateRepository();
            foreach (var entry in entries)
            {
                var sf = _sheets.Values.FirstOrDefault(f => f.Entry.Id == entry.Id);
                if (sf != null)
                {
                    sf.FormClosed -= SheetForm_FormClosed;
                    sf.Close();
                    _sheets.Remove(sf.UISheet.Id, out _);
                }
                await repo.DeleteEntry(entry);
            }
            SheetsListChanged?.Invoke(this, new GenericEventArgs<SheetForm[]>(_sheets.Values.ToArray()));
        }

        private void SheetForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            var form = sender as SheetForm;
            form.FormClosed -= SheetForm_FormClosed;
            _sheets.Remove(form.UISheet.Id, out _);
            SheetsListChanged?.Invoke(this, new GenericEventArgs<SheetForm[]>(_sheets.Values.ToArray()));
        }

        public async Task ShowSheet(Entry entry)
        {
            if (entry == null || _sheets.Any(s => s.Value.Entry.Id == entry.Id))
                return;

            var form = CreateSheetForm();
            form.StartPosition = FormStartPosition.WindowsDefaultLocation;
            form.Show();

            using var repo = Program.CreateRepository();
            var uiSheet = new UISheet
            {
                EntryId = entry.Id,
                PosX = form.Location.X,
                PosY = form.Location.Y,
                Width = form.Width,
                Height = form.Height
            };
            uiSheet.Id = await repo.CreateUISheet(uiSheet);

            form.Init(entry, uiSheet);

            _sheets[form.UISheet.Id] = form;
            SheetsListChanged?.Invoke(this, new GenericEventArgs<SheetForm[]>(_sheets.Values.ToArray()));
        }

        public async Task CloseSheet(Entry entry)
        {
            if (entry == null || !_sheets.Any(s => s.Value.Entry.Id == entry.Id))
                return;

            var form = _sheets.First(s => s.Value.Entry.Id == entry.Id);
            using var repo = Program.CreateRepository();
            await repo.DeleteUISheet(form.Value.UISheet);
            form.Value.Close();
        }

        public async Task AddNew()
        {
            using var repo = Program.CreateRepository();
            var cat = await EnsureNewCategory(repo);
            var entry = new Entry
            {
                CategoryId = cat.Id,
                CreationDate = DateTime.Now,
                Color = EntryColor.Yellow,
                Caption = DateTime.Now.ToString(),
                //Text = string.Empty //не знаю почему, но asp.net не принимает null !!!!!
            };
            entry.Id = await repo.CreateEntry(entry);

            var form = CreateSheetForm();
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Show();

            var uiSheet = new UISheet
            {
                EntryId = entry.Id,
                PosX = form.Location.X,
                PosY = form.Location.Y,
                Width = form.Width,
                Height = form.Height
            };
            uiSheet.Id = await repo.CreateUISheet(uiSheet);

            form.Init(entry, uiSheet);
            form.Focus();
            form.BringToFront();

            _sheets[form.UISheet.Id] = form;
            SheetsListChanged?.Invoke(this, new GenericEventArgs<SheetForm[]>(_sheets.Values.ToArray()));
        }

        public void RefreshAllSheets()
        {
            foreach (var sheet in _sheets.ToArray())
            {
                sheet.Value.MainFont = Settings.MainFont;
                sheet.Value.Refresh();
            }
        }

        private static async Task<Category> EnsureNewCategory(IRepository repo)
        {
            var sections = await repo.ReadSectionsEx();
            var sec = sections.FirstOrDefault(s => s.Name == SECTION_FOR_NEW);
            if (sec == null)
            {
                sec = new SectionEx { Name = SECTION_FOR_NEW };
                sec.Id = await repo.CreateSection(sec);
            }
            var cat = sec.Categories.FirstOrDefault(c => c.Name == CATEGORY_FOR_NEW);
            if (cat == null)
            {
                cat = new Category { SectionId = sec.Id, Name = CATEGORY_FOR_NEW };
                cat.Id = await repo.CreateCategory(cat);
            }
            return cat;
        }

        private SheetForm CreateSheetForm()
        {
            var form = new SheetForm();
            form.FormClosed += SheetForm_FormClosed;
            form.MainFont = Settings.MainFont;
            return form;
        }

        public void Dispose()
        {
            _alarmManger.Dispose();
        }
    }
}
