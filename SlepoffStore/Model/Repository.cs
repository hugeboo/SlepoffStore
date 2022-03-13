using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Model
{
    public sealed class Repository : IDisposable
    {
        private readonly SQLiteConnection _connection;

        public static string DatabaseName { get; set; }

        public Repository()
        {
            _connection = new SQLiteConnection("Data Source=" + DatabaseName + ";Version=3; FailIfMissing=False");
            _connection.Open();
        }

        #region Sections

        public long InsertSection(Section section)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "INSERT INTO Sections (Name) VALUES (:name)";
                command.Parameters.AddWithValue("name", section.Name);
                command.ExecuteNonQuery();
                return _connection.LastInsertRowId;
            }
        }

        public Section[] GetSections()
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "SELECT * FROM Sections";
                var data = new DataTable();
                var adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                return data.Rows.AsEnumerable().Select(r => new Section 
                {
                    Id = r.Field<long>("Id"),
                    Name = r.Field<string>("Name")
                }).ToArray();
            }
        }

        public IEnumerable<SectionEx> GetSectionsEx()
        {
            var categories = GetCategories();
            return GetSections().Select(s => new SectionEx
            {
                Id = s.Id,
                Name = s.Name,
                Categories = categories.Where(c => c.SectionId == s.Id).ToArray()
            });
        }

        #endregion

        #region Categories

        public long InsertCategory(Category category)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "INSERT INTO Categories (SectionId, Name) VALUES (:sectionId, :name)";
                command.Parameters.AddWithValue("sectionId", category.SectionId);
                command.Parameters.AddWithValue("name", category.Name);
                command.ExecuteNonQuery();
                return _connection.LastInsertRowId;
            }
        }

        public Category[] GetCategories()
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "SELECT * FROM Categories";
                var data = new DataTable();
                var adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                return data.Rows.AsEnumerable().Select(r => new Category
                {
                    Id = r.Field<long>("Id"),
                    SectionId = r.Field<long>("SectionId"),
                    Name = r.Field<string>("Name")
                }).ToArray();
            }
        }

        #endregion

        #region Entries

        public long InsertEntry(Entry entry)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "INSERT INTO Entries (CategoryId, CreationDate, Color, Caption, Text) VALUES (:categoryId, :creationDate, :color, :caption, :text)";
                command.Parameters.AddWithValue("categoryId", entry.CategoryId);
                command.Parameters.AddWithValue("creationDate", entry.CreationDate);
                command.Parameters.AddWithValue("color", entry.Color.ToString());
                command.Parameters.AddWithValue("caption", entry.Caption);
                command.Parameters.AddWithValue("text", entry.Text);
                command.ExecuteNonQuery();
                return _connection.LastInsertRowId;
            }
        }

        public Entry[] GetEntriesByCategoryId(long categoryId)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "SELECT * FROM Entries WHERE CategoryId = :categoryId";
                command.Parameters.AddWithValue("categoryId", categoryId);
                var data = new DataTable();
                var adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                return data.Rows.AsEnumerable().Select(r => new Entry
                {
                    Id = r.Field<long>("Id"),
                    CategoryId = r.Field<long>("CategoryId"),
                    CreationDate = r.Field<DateTime>("CreationDate"),
                    Color = EntryColor.Parse<EntryColor>(r.Field<string>("Color")),
                    Caption = r.Field<string>("Caption"),
                    Text = r.Field<string>("Text"),
                }).ToArray();
            }
        }

        public Entry[] GetEntriesBySectionId(long sectionId)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "SELECT Entries.* FROM Entries,Categories WHERE Entries.CategoryId=Categories.Id AND Categories.SectionId=:sectionId";
                command.Parameters.AddWithValue("sectionId", sectionId);
                var data = new DataTable();
                var adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                return data.Rows.AsEnumerable().Select(r => new Entry
                {
                    Id = r.Field<long>("Id"),
                    CategoryId = r.Field<long>("CategoryId"),
                    CreationDate = r.Field<DateTime>("CreationDate"),
                    Color = EntryColor.Parse<EntryColor>(r.Field<string>("Color")),
                    Caption = r.Field<string>("Caption"),
                    Text = r.Field<string>("Text"),
                }).ToArray();
            }
        }

        public Entry GetEntryById(long id)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "SELECT * FROM Entries WHERE Id=:id";
                command.Parameters.AddWithValue("id", id);
                var data = new DataTable();
                var adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                return data.Rows.AsEnumerable().Select(r => new Entry
                {
                    Id = r.Field<long>("Id"),
                    CategoryId = r.Field<long>("CategoryId"),
                    CreationDate = r.Field<DateTime>("CreationDate"),
                    Color = EntryColor.Parse<EntryColor>(r.Field<string>("Color")),
                    Caption = r.Field<string>("Caption"),
                    Text = r.Field<string>("Text"),
                }).FirstOrDefault();
            }
        }

        public void UpdateEntry(Entry entry)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "UPDATE Entries SET Color=:color,Caption=:caption,Text=:text WHERE Id=:id";
                command.Parameters.AddWithValue("color", entry.Color.ToString());
                command.Parameters.AddWithValue("caption", entry.Caption);
                command.Parameters.AddWithValue("text", entry.Text);
                command.Parameters.AddWithValue("id", entry.Id);
                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region UISheets

        public long InsertUISheet(UISheet sheet)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "INSERT INTO UISheets (EntryId,PosX,PosY,Width,Height) VALUES (:entryId,:posX,:posY,:width,:height)";
                command.Parameters.AddWithValue("entryId", sheet.EntryId);
                command.Parameters.AddWithValue("posX", sheet.PosX);
                command.Parameters.AddWithValue("posY", sheet.PosY);
                command.Parameters.AddWithValue("width", sheet.Width);
                command.Parameters.AddWithValue("height", sheet.Height);
                command.ExecuteNonQuery();
                return _connection.LastInsertRowId;
            }
        }

        public UISheet[] GetUISheets()
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "SELECT * FROM UISheets";
                var data = new DataTable();
                var adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                return data.Rows.AsEnumerable().Select(r => new UISheet
                {
                    Id = r.Field<long>("Id"),
                    EntryId = r.Field<long>("EntryId"),
                    PosX = (int)r.Field<long>("PosX"),
                    PosY = (int)r.Field<long>("PosY"),
                    Width = (int)r.Field<long>("Width"),
                    Height = (int)r.Field<long>("Height")
                }).ToArray();
            }
        }

        public void UpdateUISheet(UISheet sheet)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "UPDATE UISheets SET PosX=:posX,PosY=:posY,Width=:width,Height=:height WHERE Id=:id";
                command.Parameters.AddWithValue("posX", sheet.PosX);
                command.Parameters.AddWithValue("posY", sheet.PosY);
                command.Parameters.AddWithValue("width", sheet.Width);
                command.Parameters.AddWithValue("height", sheet.Height);
                command.Parameters.AddWithValue("id", sheet.Id);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteUISheet(UISheet sheet)
        {
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandText = "DELETE FROM UISheets WHERE Id=:id";
                command.Parameters.AddWithValue("id", sheet.Id);
                command.ExecuteNonQuery();
            }
        }

        #endregion

        public void Dispose()
        {
            _connection.Close();
        }

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
