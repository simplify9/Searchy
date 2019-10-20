
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

        public async Task<SearchyResponse> Search<TModel>(SearchyRequest request)
        {
            var serviceName = typeof(TModel).FullName.ToLower(); ;
            return (await Search(serviceName, request));
        }

        public async Task<SearchyResponse> Search(string serviceName, SearchyRequest request)
        {
            //var serviceName = typeof(TModel).FullName.ToLower(); ;
            var response = await Client.PostAsJsonAsync($"/api/searchy/{serviceName}", request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<SearchyResponse>();
        }

        public async Task<IEnumerable<SearchyFilterConfig>> GetFilterConfigs<TModel>()
        {
            var serviceName = typeof(TModel).FullName.ToLower(); ;
            return await GetFilterConfigs(serviceName);
        }

        public async Task<IEnumerable<SearchyFilterConfig>> GetFilterConfigs(string serviceName)
        {
            //var serviceName = typeof(TModel).FullName.ToLower(); ;
            var response = await Client.GetAsync($"/api/searchy/{serviceName}/filter");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<IEnumerable<SearchyFilterConfig>>();
        }

        //public async Task<string> Get(string lookupServiceName, object key)
        //{
        //    var response = await Client.GetAsync($"/api/lookup/{lookupServiceName}/{key}");
        //    response.EnsureSuccessStatusCode();
        //    return await response.Content.ReadAsStringAsync();
        //}


    }
}
