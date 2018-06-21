using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace HttpRequester
{
    public class HttpClient
    {
        // http://www.talkingdotnet.com/3-ways-to-use-httpclientfactory-in-asp-net-core-2-1/?utm_source=csharpdigest&utm_medium=email&utm_campaign=featured
        private readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();

        public async Task<string> GetContentAsync(string url)
        {
            return await httpClient.GetStringAsync(url);
        }
    }
}