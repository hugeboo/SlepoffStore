using SlepoffStore.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Repository
{
    public interface IRepository : IDisposable
    {
        Task<long>          CreateSection(Section section, string userName = null);
        Task<Section>       ReadSection(long id, string userName = null);
        Task<Section[]>     ReadSections(string userName = null);
        Task<SectionEx[]>   ReadSectionsEx(string userName = null);
        //void UpdateSection(Section section);
        //void DeleteSection(long sectionId);

        Task<long>          CreateCategory(Category category, string userName = null);
        Task<Category>      ReadCategory(long id, string userName = null);
        Task<Category[]>    ReadCategories(string userName = null);
        //void UpdateCategory(Category category);
        //void DeleteCategory(long categoryId);

        Task<long>          CreateEntry(Entry entry, string userName = null);
        Task<Entry[]>       ReadEntriesByCategoryId(long categoryId, string userName = null);
        Task<Entry[]>       ReadEntriesBySectionId(long sectionId, string userName = null);
        Task<Entry>         ReadEntry(long entryId, string userName = null);
        Task                UpdateEntry(Entry entry, string userName = null);
        Task                DeleteEntry(Entry entry, string userName = null);

        Task<long>          CreateUISheet(UISheet sheet, string userName = null, string deviceName = null);
        Task<UISheet[]>     ReadUISheets(string userName = null, string deviceName = null);
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
