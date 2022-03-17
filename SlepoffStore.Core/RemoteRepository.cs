using RestSharp;
using RestSharp.Serializers.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SlepoffStore.Core
{
    public sealed class RemoteRepository : IRepository
    {
        private const string API_PATH = "api/";
        private readonly RestClient _restClient;
        private string _userName;
        private string _deviceName;

        /// <summary>
        /// USE AS SINGLETON !!!
        /// </summary>
        /// <param name="url"></param>
        public RemoteRepository(string url)
        {
            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                Timeout = 50000
            };

            _restClient = new RestClient(options);

            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
            jsonOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            _restClient.UseSystemTextJson(jsonOptions);
        }

        public RemoteRepository(string url, string userName, string deviceName)
            : this(url)
        {
            _userName = userName;
            _deviceName = deviceName;
        }

        private void AddHeaders(RestRequest rr, string userName, string deviceName)
        {
            var un = string.IsNullOrWhiteSpace(userName) ? _userName : userName;
            var dn = string.IsNullOrWhiteSpace(deviceName) ? _deviceName : deviceName;

            if (!string.IsNullOrWhiteSpace(un)) rr.AddHeader("SS-UserName", un);
            if (!string.IsNullOrWhiteSpace(dn)) rr.AddHeader("SS-DeviceName", dn);
        }

        #region Sections

        public long InsertSection(Section section, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "sections").AddJsonBody(section);
            AddHeaders(rr, userName, null);
            return _restClient.PostAsync<ApiResult<long>>(rr).Result?.Data ?? 0L;
        }

        public Section[] GetSections(string userName = null)
        {
            var rr = new RestRequest(API_PATH + "sections");
            AddHeaders(rr, userName, null);
            return _restClient.GetAsync<ApiResult<Section[]>>(rr).Result?.Data;
        }

        public Section GetSection(long id, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"sections/{id}");
            AddHeaders(rr, userName, null);
            return _restClient.GetAsync<ApiResult<Section>>(rr).Result?.Data;
        }

        public IEnumerable<SectionEx> GetSectionsEx(string userName = null)
        {
            var rr = new RestRequest(API_PATH + "sections/extended");
            AddHeaders(rr, userName, null);
            return _restClient.GetAsync<ApiResult<SectionEx[]>>(rr).Result?.Data;
        }

        #endregion

        #region Categories

        public long InsertCategory(Category category, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "categories").AddJsonBody(category);
            AddHeaders(rr, userName, null);
            return _restClient.PostAsync<ApiResult<long>>(rr).Result?.Data ?? 0L;
        }

        public Category GetCategory(long id, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"categories/{id}");
            AddHeaders(rr, userName, null);
            return _restClient.GetAsync<ApiResult<Category>>(rr).Result?.Data;
        }

        public Category[] GetCategories(string userName = null)
        {
            var rr = new RestRequest(API_PATH + "categories");
            AddHeaders(rr, userName, null);
            return _restClient.GetAsync<ApiResult<Category[]>>(rr).Result?.Data;
        }

        #endregion

        #region Entries

        public long InsertEntry(Entry entry, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "entries").AddJsonBody(entry);
            AddHeaders(rr, userName, null);
            return _restClient.PostAsync<ApiResult<long>>(rr).Result?.Data ?? 0L;
        }

        public Entry GetEntry(long id, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"entries/{id}");
            AddHeaders(rr, userName, null);
            return _restClient.GetAsync<ApiResult<Entry>>(rr).Result?.Data;
        }

        public Entry[] GetEntriesByCategoryId(long categoryId, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"categories/{categoryId}/entries");
            AddHeaders(rr, userName, null);
            return _restClient.GetAsync<ApiResult<Entry[]>>(rr).Result?.Data;
        }

        public Entry[] GetEntriesBySectionId(long sectionId, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"sections/{sectionId}/entries");
            AddHeaders(rr, userName, null);
            return _restClient.GetAsync<ApiResult<Entry[]>>(rr).Result?.Data;
        }

        public void UpdateEntry(Entry entry, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "entries/update").AddJsonBody(entry);
            AddHeaders(rr, userName, null);
            var res = _restClient.PostAsync<ApiResult>(rr).Result;
        }

        #endregion

        #region KeyValues

        public void SetValue(string key, string value, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "keyvalues").AddJsonBody(new KeyValue { Key = key, Value = value });
            AddHeaders(rr, userName, null);
            var res = _restClient.PostAsync<ApiResult>(rr).Result;
        }

        public string GetValue(string key, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"keyvalues?key={key}");
            AddHeaders(rr, userName, null);
            return _restClient.GetAsync<ApiResult<string>>(rr).Result?.Data;
        }

        #endregion

        #region UISheets

        public long InsertUISheet(UISheet sheet, string userName = null, string deviceId = null)
        {
            var rr = new RestRequest(API_PATH + "uisheets").AddJsonBody(sheet);
            AddHeaders(rr, userName, deviceId);
            return _restClient.PostAsync<ApiResult<long>>(rr).Result?.Data ?? 0L;
        }

        public UISheet[] GetUISheets(string userName = null, string deviceId = null)
        {
            var rr = new RestRequest(API_PATH + "uisheets");
            AddHeaders(rr, userName, deviceId);
            return _restClient.GetAsync<ApiResult<UISheet[]>>(rr).Result?.Data;
        }

        public void UpdateUISheet(UISheet sheet, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "uisheets/update").AddJsonBody(sheet);
            AddHeaders(rr, userName, null);
            var res = _restClient.PostAsync<ApiResult>(rr).Result;
        }

        public void DeleteUISheet(UISheet sheet, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "uisheets");
            AddHeaders(rr, userName, null);
            var res = _restClient.DeleteAsync<ApiResult>(rr).Result;
        }

        #endregion

        public void Dispose()
        {
            _restClient.Dispose();
        }
    }
}
