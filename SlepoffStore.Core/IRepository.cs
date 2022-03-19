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
        Task<long>          InsertSection(Section section, string userName = null);
        Task<Section>       GetSection(long id, string userName = null);
        Task<Section[]>     GetSections(string userName = null);
        Task<SectionEx[]>   GetSectionsEx(string userName = null);
        //void UpdateSection(Section section);
        //void DeleteSection(long sectionId);

        Task<long>          InsertCategory(Category category, string userName = null);
        Task<Category>      GetCategory(long id, string userName = null);
        Task<Category[]>    GetCategories(string userName = null);
        //void UpdateCategory(Category category);
        //void DeleteCategory(long categoryId);

        Task<long>          InsertEntry(Entry entry, string userName = null);
        Task<Entry[]>       GetEntriesByCategoryId(long categoryId, string userName = null);
        Task<Entry[]>       GetEntriesBySectionId(long sectionId, string userName = null);
        Task<Entry>         GetEntry(long entryId, string userName = null);
        Task                UpdateEntry(Entry entry, string userName = null);
        //void DeleteEntry(long entryId);

        Task<long>          InsertUISheet(UISheet sheet, string userName = null, string deviceName = null);
        Task<UISheet[]>     GetUISheets(string userName = null, string deviceName = null);
        Task                UpdateUISheet(UISheet sheet, string userName = null);
        Task                DeleteUISheet(UISheet sheet, string userName = null);

        Task                SetValue(string key, string value, string userName = null);
        Task<string>        GetValue(string key, string userName = null);
    }

    public sealed class KeyValue
    {
        public string Key { get; set; }
        public string? Value { get; set; }
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
