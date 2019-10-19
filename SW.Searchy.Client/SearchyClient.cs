﻿
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SW.Searchy
{
    public class SearchyClient 
    {
        public HttpClient Client { get; }
        public SearchyClient(HttpClient client) => Client = client; 

        public async Task<IEnumerable<string>> ListAvailable()
        {
            var response = await Client.GetAsync("/api/searchy");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<IEnumerable<string>>();
        }

        public async Task<IEnumerable<TModel>> Search<TModel>(SearchyRequest request)
        {

            var serviceName = typeof(TModel).FullName.ToLower(); ;
            var response = await Client.PostAsJsonAsync($"/api/searchy/{serviceName}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<IEnumerable<TModel>>();
        }

        //public async Task<string> Get(string lookupServiceName, object key)
        //{
        //    var response = await Client.GetAsync($"/api/lookup/{lookupServiceName}/{key}");
        //    response.EnsureSuccessStatusCode();
        //    return await response.Content.ReadAsStringAsync();
        //}


    }
}