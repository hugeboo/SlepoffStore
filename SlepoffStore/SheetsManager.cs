using SlepoffStore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore
{
    public sealed class SheetsManager
    {
        private const string SECTION_FOR_NEW = "Other";
        private const string CATEGORY_FOR_NEW = "New";

        private readonly List<SheetForm> _sheets = new List<SheetForm>();

        public void CollapseAllSheets()
        {
            foreach (var sheet in _sheets)
            {
                sheet.FormClosed -= Form_FormClosed;
                sheet.Close();
            }
            _sheets.Clear();
        }

        public void RestoreAllSheets()
        {
            using var repo = new Repository();
            var uiSheets = repo.GetUISheets();
            
            foreach (var uiSheet in uiSheets)
            {
                if (!_sheets.Any(s => s.UISheet.Id == uiSheet.Id))
                {
                    var form = CreateSheetForm();
                    form.Size = new Size(uiSheet.Width, uiSheet.Height);
                    form.Location = new Point(uiSheet.PosX, uiSheet.PosY);
                    form.Show();
                    form.Init(repo.GetEntryById(uiSheet.EntryId), uiSheet);
                    _sheets.Add(form);
                }
            }
        }

        private void Form_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _sheets.Remove(sender as SheetForm);
        }

        public void ShowSheet(Entry entry)
        {
            if (entry == null || _sheets.Any(s => s.Entry.Id == entry.Id)) 
                return;

            var form = CreateSheetForm();
            form.StartPosition = FormStartPosition.WindowsDefaultLocation;
            form.Show();

            using var repo = new Repository();
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

        public void CloseSheet(Entry entry)
        {
            if (entry == null || !_sheets.Any(s => s.Entry.Id == entry.Id))
                return;

            var form = _sheets.First(s => s.Entry.Id == entry.Id);
            using var repo = new Repository();
            repo.DeleteUISheet(form.UISheet);
            form.Close();
        }

        public void AddNew()
        {
            using var repo = new Repository();
            var cat = EnsureNewCategory(repo);
            var entry = new Entry 
            { 
                CategoryId = cat.Id, 
                CreationDate = DateTime.Now,
                Color = EntryColor.Yellow,
                Caption = DateTime.Now.ToString() 
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

            _sheets.Add(form);
        }

        public void RefreshAllSheets()
        {
            foreach (var sheet in _sheets)
            {
                sheet.MainFont = Settings.MainFont;
                sheet.Refresh();
            }
        }

        private Category EnsureNewCategory(Repository repo)
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
    }
}
