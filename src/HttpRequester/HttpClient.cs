using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace HttpRequester
{
    public class HttpClient
    {
        private readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();

        public async Task<string> GetContentAsync(string url)
        {
            return await httpClient.GetStringAsync(url);
        }
    }
}