using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;
using SlepoffStore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStoreApp.Repository
{
    public sealed class RemoteRepository
    {
        private readonly ISlepoffStoreHttpApi _httpApi;

        public RemoteRepository(string url, string userName, string password, string deviceName)
        {
            var httpClientHandler = new HttpClientHandler() 
            {
                ServerCertificateCustomValidationCallback = (message, certificate, chain, sslPolicyErrors) => true
            };

            var authHeader = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + password));

            var client = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(url),
                DefaultRequestHeaders =
                {
                    {"Authorization", $"Basic {authHeader}"},
                    {"SS-UserName", userName },
                    {"SS-DeviceName", deviceName },
                },
            };

            var settings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(new Newtonsoft.Json.JsonSerializerSettings 
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new StringEnumConverter() }
                }),
            };

            _httpApi = RestService.For<ISlepoffStoreHttpApi>(client, settings);
        }

        public async Task<Section[]> GetSections()
        {
            return (await _httpApi.GetSections())?.Data;
        }
    }
}