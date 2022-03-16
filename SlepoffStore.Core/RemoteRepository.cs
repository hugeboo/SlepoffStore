using System;
using System.Collections.Generic;
using System.Text;

namespace SlepoffStore.Core
{
    public sealed class RemoteRepository : IRepository
    {
        public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void DeleteUISheet(UISheet sheet)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Category[] GetCategories()
        {
            throw new NotImplementedException();
        }

        public Entry[] GetEntriesByCategoryId(long categoryId)
        {
            throw new NotImplementedException();
        }

        public Entry[] GetEntriesBySectionId(long sectionId)
        {
            throw new NotImplementedException();
        }

        public Entry GetEntry(long entryId)
        {
            throw new NotImplementedException();
        }

        public Section[] GetSections()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SectionEx> GetSectionsEx()
        {
            throw new NotImplementedException();
        }

        public UISheet[] GetUISheets()
        {
            throw new NotImplementedException();
        }

        public long InsertCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public long InsertEntry(Entry entry)
        {
            throw new NotImplementedException();
        }

        public long InsertSection(Section section)
        {
            throw new NotImplementedException();
        }

        public long InsertUISheet(UISheet sheet)
        {
            throw new NotImplementedException();
        }

        public void UpdateEntry(Entry entry)
        {
            throw new NotImplementedException();
        }

        public void UpdateUISheet(UISheet sheet)
        {
            throw new NotImplementedException();
        }
    }
}
