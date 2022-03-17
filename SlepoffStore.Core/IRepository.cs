using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Core
{
    public interface IRepository : IDisposable
    {
        long InsertSection(Section section, string userName = null);
        Section GetSection(long id, string userName = null);
        Section[] GetSections(string userName = null);
        IEnumerable<SectionEx> GetSectionsEx(string userName = null);
        //void UpdateSection(Section section);
        //void DeleteSection(long sectionId);

        long InsertCategory(Category category, string userName = null);
        Category GetCategory(long id, string userName = null);
        Category[] GetCategories(string userName = null);
        //void UpdateCategory(Category category);
        //void DeleteCategory(long categoryId);

        long InsertEntry(Entry entry, string userName = null);
        Entry[] GetEntriesByCategoryId(long categoryId, string userName = null);
        Entry[] GetEntriesBySectionId(long sectionId, string userName = null);
        Entry GetEntry(long entryId, string userName = null);
        void UpdateEntry(Entry entry, string userName = null);
        //void DeleteEntry(long entryId);

        long InsertUISheet(UISheet sheet, string userName = null, string deviceName = null);
        UISheet[] GetUISheets(string userName = null, string deviceName = null);
        void UpdateUISheet(UISheet sheet, string userName = null);
        void DeleteUISheet(UISheet sheet, string userName = null);

        void SetValue(string key, string value, string userName = null);
        string GetValue(string key, string userName = null);
    }

    public sealed class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    internal static class RepositoryExtensions
    {
        public static IEnumerable<DataRow> AsEnumerable(this DataRowCollection rows)
        {
            foreach (DataRow row in rows)
            {
                yield return row;
            }
        }
    }
}
