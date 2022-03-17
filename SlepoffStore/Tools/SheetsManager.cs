using SlepoffStore.Core;
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

        private readonly List<SheetForm> _sheets = new List<SheetForm>();
        private readonly AlarmManager _alarmManger;

        public SheetsManager()
        {
            _alarmManger = new AlarmManager(this);
        }

        public SheetForm[] Sheets
        {
            get
            {
                lock (_sheets) return _sheets.ToArray();
            }
        }

        public bool Collapsed { get; private set; }

        public void CollapseAllSheets()
        {
            lock (_sheets)
            {
                foreach (var sheet in _sheets)
                {
                    sheet.FormClosed -= Form_FormClosed;
                    sheet.Close();
                }
                _sheets.Clear();
                Collapsed = true;
            }
        }

        public void RestoreAllSheets()
        {
            lock (_sheets)
            {
                using var repo = Program.CreateRepository();
                var uiSheets = repo.GetUISheets();

                var focus = GetForegroundWindow();

                foreach (var uiSheet in uiSheets)
                {
                    if (!_sheets.Any(s => s.UISheet.Id == uiSheet.Id))
                    {
                        var form = CreateSheetForm();
                        form.Size = new Size(uiSheet.Width, uiSheet.Height);
                        form.Location = new Point(uiSheet.PosX, uiSheet.PosY);
                        form.Show();
                        form.Init(repo.GetEntry(uiSheet.EntryId), uiSheet);
                        _sheets.Add(form);
                    }
                }

                SetForegroundWindow(focus);
                Collapsed = false;
            }
        }

        private void Form_FormClosed(object? sender, FormClosedEventArgs e)
        {
            lock (_sheets) _sheets.Remove(sender as SheetForm);
        }

        public void ShowSheet(Entry entry)
        {
            lock (_sheets)
            {
                if (entry == null || _sheets.Any(s => s.Entry.Id == entry.Id))
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
                uiSheet.Id = repo.InsertUISheet(uiSheet);

                form.Init(entry, uiSheet);

                _sheets.Add(form);
            }
        }

        public void CloseSheet(Entry entry)
        {
            lock (_sheets)
            {
                if (entry == null || !_sheets.Any(s => s.Entry.Id == entry.Id))
                    return;

                var form = _sheets.First(s => s.Entry.Id == entry.Id);
                using var repo = Program.CreateRepository();
                repo.DeleteUISheet(form.UISheet);
                form.Close();
            }
        }

        public void AddNew()
        {
            using var repo = Program.CreateRepository();
            var cat = EnsureNewCategory(repo);
            var entry = new Entry 
            { 
                CategoryId = cat.Id, 
                CreationDate = DateTime.Now,
                Color = EntryColor.Yellow,
                Caption = DateTime.Now.ToString(),
                //Text = string.Empty //не знаю почему, но asp.net не принимает null !!!!!
            };
            entry.Id = repo.InsertEntry(entry);

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
            uiSheet.Id = repo.InsertUISheet(uiSheet);

            form.Init(entry, uiSheet);
            form.Focus();
            form.BringToFront();

            lock (_sheets) _sheets.Add(form);
        }

        public void RefreshAllSheets()
        {
            lock (_sheets)
            {
                foreach (var sheet in _sheets)
                {
                    sheet.MainFont = Settings.MainFont;
                    sheet.Refresh();
                }
            }
        }

        private Category EnsureNewCategory(IRepository repo)
        {
            var sections = repo.GetSectionsEx();
            var sec = sections.FirstOrDefault(s => s.Name == SECTION_FOR_NEW);
            if (sec == null)
            {
                sec = new SectionEx { Name = SECTION_FOR_NEW };
                sec.Id = repo.InsertSection(sec);
            }
            var cat = sec.Categories.FirstOrDefault(c => c.Name == CATEGORY_FOR_NEW);
            if (cat == null)
            {
                cat = new Category { SectionId = sec.Id, Name = CATEGORY_FOR_NEW };
                cat.Id = repo.InsertCategory(cat);
            }
            return cat;
        }

        private SheetForm CreateSheetForm()
        {
            var form = new SheetForm();
            form.FormClosed += Form_FormClosed;
            form.MainFont = Settings.MainFont;
            return form;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
