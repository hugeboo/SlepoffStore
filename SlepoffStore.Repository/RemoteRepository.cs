using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Json;
using SlepoffStore.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SlepoffStore.Repository
{
    public sealed class RemoteRepository : IRepository
    {
        private const string API_PATH = "api/";
        private readonly RestClient _restClient;
        private readonly string _deviceName;
        private readonly string _userName;

        /// <summary>
        /// USE AS SINGLETON !!! ???
        /// </summary>
        public RemoteRepository(string url, string userName, string password, string deviceName)
        {
            _deviceName = deviceName;
            _userName = userName;

            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                Timeout = 5000,

                // !!! ONLY for debug
                //RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            _restClient = new RestClient(options)
            {
                Authenticator = new HttpBasicAuthenticator(userName, password)
            };

            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
            jsonOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            _restClient.UseSystemTextJson(jsonOptions);
        }

        public void Dispose()
        {
            _restClient.Dispose();
        }

        #region Sections

        public async Task<long> CreateSection(Section section, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "sections").AddJsonBody(section);
            AddHeaders(rr, userName, null);
            return (await _restClient.PostAsync<ApiResult<long>>(rr))?.Data ?? 0L;
        }

        public async Task<Section[]> ReadSections(string userName = null)
        {
            var rr = new RestRequest(API_PATH + "sections");
            AddHeaders(rr, userName, null);
            return (await _restClient.GetAsync<ApiResult<Section[]>>(rr))?.Data;
        }

        public async Task<Section> ReadSection(long id, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"sections/{id}");
            AddHeaders(rr, userName, null);
            return (await _restClient.GetAsync<ApiResult<Section>>(rr))?.Data;
        }

        public async Task<SectionEx[]> ReadSectionsEx(string userName = null)
        {
            var rr = new RestRequest(API_PATH + "sections/extended");
            AddHeaders(rr, userName, null);
            return (await _restClient.GetAsync<ApiResult<SectionEx[]>>(rr))?.Data;
        }

        #endregion

        #region Categories

        public async Task<long> CreateCategory(Category category, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "categories").AddJsonBody(category);
            AddHeaders(rr, userName, null);
            return (await _restClient.PostAsync<ApiResult<long>>(rr))?.Data ?? 0L;
        }

        public async Task<Category> ReadCategory(long id, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"categories/{id}");
            AddHeaders(rr, userName, null);
            return (await _restClient.GetAsync<ApiResult<Category>>(rr))?.Data;
        }

        public async Task<Category[]> ReadCategories(string userName = null)
        {
            var rr = new RestRequest(API_PATH + "categories");
            AddHeaders(rr, userName, null);
            return (await _restClient.GetAsync<ApiResult<Category[]>>(rr))?.Data;
        }

        #endregion

        #region Entries

        public async Task<long> CreateEntry(Entry entry, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "entries").AddJsonBody(entry);
            AddHeaders(rr, userName, null);
            return (await _restClient.PostAsync<ApiResult<long>>(rr))?.Data ?? 0L;
        }

        public async Task<Entry> ReadEntry(long id, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"entries/{id}");
            AddHeaders(rr, userName, null);
            return (await _restClient.GetAsync<ApiResult<Entry>>(rr))?.Data;
        }

        public async Task<Entry[]> ReadEntriesByCategoryId(long categoryId, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"categories/{categoryId}/entries");
            AddHeaders(rr, userName, null);
            return (await _restClient.GetAsync<ApiResult<Entry[]>>(rr))?.Data;
        }

        public async Task<Entry[]> ReadEntriesBySectionId(long sectionId, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"sections/{sectionId}/entries");
            AddHeaders(rr, userName, null);
            return (await _restClient.GetAsync<ApiResult<Entry[]>>(rr))?.Data;
        }

        public async Task UpdateEntry(Entry entry, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "entries/update").AddJsonBody(entry);
            AddHeaders(rr, userName, null);
            var res = await _restClient.PostAsync<ApiResult>(rr);
        }

        public async Task DeleteEntry(Entry entry, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "entries/delete").AddJsonBody(entry);
            AddHeaders(rr, userName, null);
            var res = await _restClient.PostAsync<ApiResult>(rr);
        }

        #endregion

        #region KeyValues

        public async Task SetValue(string key, string value, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "keyvalues").AddJsonBody(new KeyValue { Key = key, Value = value });
            AddHeaders(rr, userName, null);
            var res = await _restClient.PostAsync<ApiResult>(rr);
        }

        public async Task<string> GetValue(string key, string userName = null)
        {
            var rr = new RestRequest(API_PATH + $"keyvalues?key={key}");
            AddHeaders(rr, userName, null);
            return (await _restClient.GetAsync<ApiResult<string>>(rr))?.Data;
        }

        #endregion

        #region UISheets

        public async Task<long> CreateUISheet(UISheet sheet, string userName = null, string deviceId = null)
        {
            var rr = new RestRequest(API_PATH + "uisheets").AddJsonBody(sheet);
            AddHeaders(rr, userName, deviceId);
            return (await _restClient.PostAsync<ApiResult<long>>(rr))?.Data ?? 0L;
        }

        public async Task<UISheet[]> ReadUISheets(string userName = null, string deviceId = null)
        {
            var rr = new RestRequest(API_PATH + "uisheets");
            AddHeaders(rr, userName, deviceId);
            return (await _restClient.GetAsync<ApiResult<UISheet[]>>(rr))?.Data;
        }

        public async Task UpdateUISheet(UISheet sheet, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "uisheets/update").AddJsonBody(sheet);
            AddHeaders(rr, userName, null);
            var res = await _restClient.PostAsync<ApiResult>(rr);
        }

        public async Task DeleteUISheet(UISheet sheet, string userName = null)
        {
            var rr = new RestRequest(API_PATH + "uisheets").AddJsonBody(sheet);
            AddHeaders(rr, userName, null);
            var res = await _restClient.DeleteAsync<ApiResult>(rr);
        }

        #endregion

        private void AddHeaders(RestRequest rr, string userName, string deviceName)
        {
            var un = string.IsNullOrWhiteSpace(userName) ? _userName : userName;
            var dn = string.IsNullOrWhiteSpace(deviceName) ? _deviceName : deviceName;

            if (!string.IsNullOrWhiteSpace(un)) rr.AddHeader("SS-UserName", un);
            if (!string.IsNullOrWhiteSpace(dn)) rr.AddHeader("SS-DeviceName", dn);
        }
    }
}
