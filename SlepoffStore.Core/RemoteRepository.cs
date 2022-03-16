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
            throw new NotImplementedException();
        }

        public Section[] GetSections()
        {
            var rr = new RestRequest(API_PATH + "sections");
            return _restClient.GetAsync<ApiResult<Section[]>>(rr).Result?.Data;
        }

        public IEnumerable<SectionEx> GetSectionsEx()
        {
            throw new NotImplementedException();
        }

        #endregion

        public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void DeleteUISheet(UISheet sheet)
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

        public void Dispose()
        {
            _restClient.Dispose();
        }
    }
}
