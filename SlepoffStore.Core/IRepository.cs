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
        long InsertSection(Section section);
        Section[] GetSections();
        IEnumerable<SectionEx> GetSectionsEx();
        //void UpdateSection(Section section);
        //void DeleteSection(long sectionId);

        long InsertCategory(Category category);
        Category[] GetCategories();
        //void UpdateCategory(Category category);
        //void DeleteCategory(long categoryId);

        long InsertEntry(Entry entry);
        Entry[] GetEntriesByCategoryId(long categoryId);
        Entry[] GetEntriesBySectionId(long sectionId);
        Entry GetEntry(long entryId);
        void UpdateEntry(Entry entry);
        //void DeleteEntry(long entryId);

        string this[string key] { get; set; }
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
