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

        /// <summary>
        /// USE AS SINGLETON !!!
        /// </summary>
        /// <param name="url"></param>
        public RemoteRepository(string url)
        {
            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                Timeout = 5000
            };

            _restClient = new RestClient(options);

            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
            jsonOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            _restClient.UseSystemTextJson(jsonOptions);
        }

        #region Sections

        public long InsertSection(Section section)
        {
            var rr = new RestRequest(API_PATH + "sections").AddJsonBody(section);
            return _restClient.PostAsync<ApiResult<long>>(rr).Result?.Data ?? 0L;
        }

        public Section[] GetSections()
        {
            var rr = new RestRequest(API_PATH + "sections");
            return _restClient.GetAsync<ApiResult<Section[]>>(rr).Result?.Data;
        }

        public IEnumerable<SectionEx> GetSectionsEx()
        {
            var rr = new RestRequest(API_PATH + "sections/extended");
            return _restClient.GetAsync<ApiResult<SectionEx[]>>(rr).Result?.Data;
        }

        #endregion

        #region Categories

        public long InsertCategory(Category category)
        {
            var rr = new RestRequest(API_PATH + "categories").AddJsonBody(category);
            return _restClient.PostAsync<ApiResult<long>>(rr).Result?.Data ?? 0L;
        }

        public Category[] GetCategories()
        {
            var rr = new RestRequest(API_PATH + "categories");
            return _restClient.GetAsync<ApiResult<Category[]>>(rr).Result?.Data;
        }

        #endregion

        #region Entries

        public long InsertEntry(Entry entry)
        {
            var rr = new RestRequest(API_PATH + "entries").AddJsonBody(entry);
            return _restClient.PostAsync<ApiResult<long>>(rr).Result?.Data ?? 0L;
        }

        public Entry GetEntry(long id)
        {
            var rr = new RestRequest(API_PATH + $"entries/{id}");
            return _restClient.GetAsync<ApiResult<Entry>>(rr).Result?.Data;
        }

        public Entry[] GetEntriesByCategoryId(long categoryId)
        {
            var rr = new RestRequest(API_PATH + $"categories/{categoryId}/entries");
            return _restClient.GetAsync<ApiResult<Entry[]>>(rr).Result?.Data;
        }

        public Entry[] GetEntriesBySectionId(long sectionId)
        {
            var rr = new RestRequest(API_PATH + $"sections/{sectionId}/entries");
            return _restClient.GetAsync<ApiResult<Entry[]>>(rr).Result?.Data;
        }

        public void UpdateEntry(Entry entry)
        {
            var rr = new RestRequest(API_PATH + "entries/update").AddJsonBody(entry);
            var res = _restClient.PostAsync<ApiResult>(rr).Result;
        }

        #endregion

        #region KeyValues

        public string this[string key] 
        {
            get
            {
                var rr = new RestRequest(API_PATH + $"keyvalues?key={key}");
                return _restClient.GetAsync<ApiResult<string>>(rr).Result?.Data;
            }
            set
            {
                var rr = new RestRequest(API_PATH + "keyvalues").AddJsonBody(new KeyValue { Key = key, Value = value });
                var res = _restClient.PostAsync<ApiResult>(rr).Result;
            }
        }

        #endregion

        #region UISheets

        public long InsertUISheet(UISheet sheet)
        {
            throw new NotImplementedException();
        }
 
        public UISheet[] GetUISheets()
        {
            throw new NotImplementedException();
        }
 
        public void UpdateUISheet(UISheet sheet)
        {
            throw new NotImplementedException();
        }

        public void DeleteUISheet(UISheet sheet)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Dispose()
        {
            _restClient.Dispose();
        }
    }
}
