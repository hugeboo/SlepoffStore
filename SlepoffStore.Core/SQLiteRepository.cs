using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Core
{
    public sealed class SQLiteRepository : IRepository, IUserRepository
    {
        private readonly SQLiteConnection _connection;
        private readonly string _userName;
        private readonly string _deviceName;

        public SQLiteRepository(string databaseName)
        {
            _connection = new SQLiteConnection("Data Source=" + databaseName + ";Version=3; FailIfMissing=False");
            _connection.Open();
        }

        public SQLiteRepository(string databaseName, string userName, string deviceName)
            : this(databaseName)
        {
            _userName = userName;
            _deviceName = deviceName;
        }

        public void Dispose()
        {
            _connection.Close();
        }

        #region Sections

        public async Task<long> InsertSection(Section section, string userName = null)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText = 
                "INSERT INTO Sections(Name, UserId) SELECT :name, Id FROM Users WHERE Name=:userName LIMIT 1";
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            command.Parameters.AddWithValue("name", section.Name);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<Section> GetSection(long id, string userName = null)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText =
                "SELECT Sections.* FROM Sections " +
                "INNER JOIN Users ON Sections.UserId = Users.Id AND Sections.Id=:id AND Users.Name = :userName";
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            command.Parameters.AddWithValue("id", id);
            var data = await command.ExecuteQueryAsync();
            return data.Rows.AsEnumerable().Select(r => new Section
            {
                Id = r.Field<long>("Id"),
                Name = r.Field<string>("Name")
            }).FirstOrDefault();
        }

        public async Task<Section[]> GetSections(string userName = null)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText =
                "SELECT Sections.* FROM Sections " +
                "INNER JOIN Users ON Sections.UserId = Users.Id AND Users.Name = :userName";
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            var data = await command.ExecuteQueryAsync();
            return data.Rows.AsEnumerable().Select(r => new Section
            {
                Id = r.Field<long>("Id"),
                Name = r.Field<string>("Name")
            }).ToArray();
        }

        public async Task<SectionEx[]> GetSectionsEx(string userName = null)
        {
            var categories = await GetCategories(userName);
            var sections = await GetSections(userName);
            return sections.Select(s => new SectionEx
            {
                Id = s.Id,
                Name = s.Name,
                Categories = categories.Where(c => c.SectionId == s.Id).ToArray()
            }).ToArray();
        }

        #endregion

        #region Categories

        public async Task<long> InsertCategory(Category category, string userName = null)
        {
            var section = GetSection(category.SectionId, userName);
            if (section == null) throw new Exception("Category is unavailable");

            using var command = new SQLiteCommand(_connection);
            command.CommandText = "INSERT INTO Categories (SectionId, Name) VALUES (:sectionId, :name)";
            command.Parameters.AddWithValue("sectionId", category.SectionId);
            command.Parameters.AddWithValue("name", category.Name);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<Category> GetCategory(long id, string userName = null)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText =
                "SELECT Categories.* FROM Categories " +
                "INNER JOIN Sections ON Sections.Id = Categories.SectionId AND Categories.Id=:id " +
                "INNER JOIN Users ON Sections.UserId = Users.Id AND Users.Name = :userName";
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            command.Parameters.AddWithValue("id", id);
            var data = await command.ExecuteQueryAsync();
            return data.Rows.AsEnumerable().Select(r => new Category
            {
                Id = r.Field<long>("Id"),
                SectionId = r.Field<long>("SectionId"),
                Name = r.Field<string>("Name")
            }).FirstOrDefault();
        }

        public async Task<Category[]> GetCategories(string userName = null)
        {
            var command = new SQLiteCommand(_connection);
            command.CommandText =
                "SELECT Categories.* FROM Categories " + 
                "INNER JOIN Sections ON Sections.Id = Categories.SectionId " +
                "INNER JOIN Users ON Sections.UserId = Users.Id AND Users.Name = :userName";
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            var data = await command.ExecuteQueryAsync();
            return data.Rows.AsEnumerable().Select(r => new Category
            {
                Id = r.Field<long>("Id"),
                SectionId = r.Field<long>("SectionId"),
                Name = r.Field<string>("Name")
            }).ToArray();
        }

        #endregion

        #region Entries

        public async Task<long> InsertEntry(Entry entry, string userName = null)
        {
            var category = GetCategory(entry.CategoryId, userName);
            if (category == null) throw new Exception("Category is unavailable");

            using var command = new SQLiteCommand(_connection);
            command.CommandText = "INSERT INTO Entries (CategoryId, CreationDate, Color, Caption, Text, Alarm, AlarmIsOn) VALUES (:categoryId, :creationDate, :color, :caption, :text, :alarm, :alarmIsOn)";
            command.Parameters.AddWithValue("categoryId", entry.CategoryId);
            command.Parameters.AddWithValue("creationDate", entry.CreationDate);
            command.Parameters.AddWithValue("color", entry.Color.ToString());
            command.Parameters.AddWithValue("caption", entry.Caption);
            command.Parameters.AddWithValue("text", entry.Text);
            command.Parameters.AddWithValue("alarm", entry.Alarm);
            command.Parameters.AddWithValue("alarmIsOn", entry.AlarmIsOn);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<Entry[]> GetEntriesByCategoryId(long categoryId, string userName = null)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText = "SELECT Entries.* FROM Entries " +
                "INNER JOIN Categories ON Categories.Id = Entries.CategoryId AND Categories.Id = :categoryId " +
                "INNER JOIN Sections ON Sections.Id = Categories.SectionId " +
                "INNER JOIN Users ON Sections.UserId = Users.Id AND Users.Name = :userName";
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            command.Parameters.AddWithValue("categoryId", categoryId);
            var data = await command.ExecuteQueryAsync();
            return data.Rows.AsEnumerable().Select(r => new Entry
            {
                Id = r.Field<long>("Id"),
                CategoryId = r.Field<long>("CategoryId"),
                CreationDate = r.Field<DateTime>("CreationDate"),
                Color = EntryColor.Parse<EntryColor>(r.Field<string>("Color")),
                Caption = r.Field<string>("Caption"),
                Text = r.Field<string>("Text"),
                Alarm = r.Field<DateTime?>("Alarm"),
                AlarmIsOn = r.Field<bool>("AlarmIsOn"),
            }).ToArray();
        }

        public async Task<Entry[]> GetEntriesBySectionId(long sectionId, string userName = null)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText = "SELECT Entries.* FROM Entries " +
                "INNER JOIN Categories ON Categories.Id = Entries.CategoryId " +
                "INNER JOIN Sections ON Sections.Id = Categories.SectionId AND Sections.Id=:sectionId " +
                "INNER JOIN Users ON Sections.UserId = Users.Id AND Users.Name = :userName";
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            command.Parameters.AddWithValue("sectionId", sectionId);
            var data = await command.ExecuteQueryAsync();
            return data.Rows.AsEnumerable().Select(r => new Entry
            {
                Id = r.Field<long>("Id"),
                CategoryId = r.Field<long>("CategoryId"),
                CreationDate = r.Field<DateTime>("CreationDate"),
                Color = EntryColor.Parse<EntryColor>(r.Field<string>("Color")),
                Caption = r.Field<string>("Caption"),
                Text = r.Field<string>("Text"),
                Alarm = r.Field<DateTime?>("Alarm"),
                AlarmIsOn = r.Field<bool>("AlarmIsOn"),
            }).ToArray();
        }

        public async Task<Entry> GetEntry(long id, string userName = null)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText = "SELECT Entries.* FROM Entries " +
                "INNER JOIN Categories ON Categories.Id = Entries.CategoryId AND Entries.Id=:id " +
                "INNER JOIN Sections ON Sections.Id = Categories.SectionId " +
                "INNER JOIN Users ON Sections.UserId = Users.Id AND Users.Name = :userName";
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            command.Parameters.AddWithValue("id", id);
            var data = await command.ExecuteQueryAsync();
            return data.Rows.AsEnumerable().Select(r => new Entry
            {
                Id = r.Field<long>("Id"),
                CategoryId = r.Field<long>("CategoryId"),
                CreationDate = r.Field<DateTime>("CreationDate"),
                Color = EntryColor.Parse<EntryColor>(r.Field<string>("Color")),
                Caption = r.Field<string>("Caption"),
                Text = r.Field<string>("Text"),
                Alarm = r.Field<DateTime?>("Alarm"),
                AlarmIsOn = r.Field<bool>("AlarmIsOn"),
            }).FirstOrDefault();
        }

        public async Task UpdateEntry(Entry entry, string userName = null)
        {
            var category = GetCategory(entry.CategoryId, userName);
            if (category == null) throw new Exception("Category is unavailable");

            using var command = new SQLiteCommand(_connection);
            command.CommandText = "UPDATE Entries SET Color=:color,Caption=:caption,Text=:text,Alarm=:alarm,AlarmIsOn=:alarmIsOn WHERE Id=:id";
            command.Parameters.AddWithValue("color", entry.Color.ToString());
            command.Parameters.AddWithValue("caption", entry.Caption);
            command.Parameters.AddWithValue("text", entry.Text);
            command.Parameters.AddWithValue("alarm", entry.Alarm);
            command.Parameters.AddWithValue("alarmIsOn", entry.AlarmIsOn);
            command.Parameters.AddWithValue("id", entry.Id);
            await command.ExecuteNonQueryAsync();
        }

        #endregion

        #region UISheets

        public async Task<long> InsertUISheet(UISheet sheet, string userName = null, string deviceName = null)
        {
            var entry = GetEntry(sheet.EntryId, userName);
            if (entry == null) throw new Exception("Entry is unavailable");

            using var command = new SQLiteCommand(_connection);
            command.CommandText =
                "INSERT INTO UISheets (EntryId,DeviceId,PosX,PosY,Width,Height) " +
                "SELECT :entryId,Devices.Id,:posX,:posY,:width,:height FROM Devices " +
                "INNER JOIN Users ON Devices.UserId = Users.Id AND Users.Name = :userName AND Devices.Name=:deviceName " +
                "LIMIT 1";
            command.Parameters.AddWithValue("entryId", sheet.EntryId);
            command.Parameters.AddWithValue("deviceName", deviceName ?? _deviceName);
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            command.Parameters.AddWithValue("posX", sheet.PosX);
            command.Parameters.AddWithValue("posY", sheet.PosY);
            command.Parameters.AddWithValue("width", sheet.Width);
            command.Parameters.AddWithValue("height", sheet.Height);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<UISheet[]> GetUISheets(string userName = null, string deviceName = null)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText = "SELECT UISheets.* FROM UISheets " +
                "INNER JOIN Entries ON Entries.Id = UISheets.EntryId " +
                "INNER JOIN Categories ON Categories.Id = Entries.CategoryId " +
                "INNER JOIN Sections ON Sections.Id = Categories.SectionId " +
                "INNER JOIN Users ON Sections.UserId = Users.Id AND Users.Name = :userName " +
                "INNER JOIN Devices ON Devices.UserId = Users.Id AND Devices.Name = :deviceName";
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            command.Parameters.AddWithValue("deviceName", deviceName ?? _deviceName);
            var data = await command.ExecuteQueryAsync();
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

        public async Task UpdateUISheet(UISheet sheet, string userName = null)
        {
            var entry = GetEntry(sheet.EntryId, userName);
            if (entry == null) throw new Exception("Entry is unavailable");

            using var command = new SQLiteCommand(_connection);
            command.CommandText = "UPDATE UISheets SET PosX=:posX,PosY=:posY,Width=:width,Height=:height WHERE Id=:id";
            command.Parameters.AddWithValue("posX", sheet.PosX);
            command.Parameters.AddWithValue("posY", sheet.PosY);
            command.Parameters.AddWithValue("width", sheet.Width);
            command.Parameters.AddWithValue("height", sheet.Height);
            command.Parameters.AddWithValue("id", sheet.Id);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteUISheet(UISheet sheet, string userName = null)
        {
            var entry = GetEntry(sheet.EntryId, userName);
            if (entry == null) throw new Exception("Entry is unavailable");

            using var command = new SQLiteCommand(_connection);
            command.CommandText = "DELETE FROM UISheets WHERE Id=:id";
            command.Parameters.AddWithValue("id", sheet.Id);
            await command.ExecuteNonQueryAsync();
        }

        #endregion

        #region KeyValues

        public async Task SetValue(string key, string value, string userName = null)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText = 
                "INSERT INTO KeyValues (Key, Value, UserId) " +
                "SELECT :key,:value,Id FROM Users WHERE Name=:userName LIMIT 1 " +
                "ON CONFLICT (Key,UserId) DO UPDATE SET Value = excluded.Value";
            command.Parameters.AddWithValue("key", key);
            command.Parameters.AddWithValue("value", value);
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<string> GetValue(string key, string userName = null)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText = 
                "SELECT KeyValues.* FROM KeyValues " +
                "INNER JOIN Users ON KeyValues.UserId=Users.Id AND KeyValues.Key=:key AND Users.Name=:userName";
            command.Parameters.AddWithValue("key", key);
            command.Parameters.AddWithValue("userName", userName ?? _userName);
            var data = await command.ExecuteQueryAsync();
            return data.Rows.AsEnumerable().Select(r => r.Field<string>("Value")).FirstOrDefault();
        }

        #endregion

        #region Users

        public async Task<User> GetUser(string username)
        {
            using var command = new SQLiteCommand(_connection);
            command.CommandText = "SELECT * FROM Users WHERE Name=:name";
            command.Parameters.AddWithValue("name", username);
            var data = await command.ExecuteQueryAsync();
            return data.Rows.AsEnumerable().Select(r => new User
            {
                Id = r.Field<long>("Id"),
                Name = r.Field<string>("Name"),
                Password = r.Field<string>("Password"),
                Comments = r.Field<string>("Comments"),
            }).FirstOrDefault();
        }

        #endregion
    }

    public static class SQLiteExtensions
    {
        private static readonly object _syncObj = new object();

        public static async Task<long> ExecuteNonQueryAsync(this SQLiteCommand command)
        {
            return await Task.Factory.StartNew(() => 
            {
                lock (_syncObj)
                {
                    var transaction = command.Connection.BeginTransaction();
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                        var lastId = command.Connection.LastInsertRowId;
                        transaction.Commit();
                        return lastId;
                }
            });
        }

        public static async Task<DataTable> ExecuteQueryAsync(this SQLiteCommand command)
        {
            return await Task.Factory.StartNew(() =>
            {
                lock (_syncObj)
                {
                    using var transaction = command.Connection.BeginTransaction();
                         command.Transaction = transaction;
                        var data = new DataTable();
                        using var adapter = new SQLiteDataAdapter(command);
                        adapter.Fill(data);
                        transaction.Commit();
                        return data;
                }
            });
        }
    }
}
