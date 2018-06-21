using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public class DownloaderMiddleware
    {
        public string RedisConnectrionstring { get; set; }
        public bool UseRedisCache { get; set; }

        public async Task<string> RunAsync(string url)
        {
            if (UseRedisCache)
            {
                HttpRequester.HttpCachedClient client = new HttpRequester.HttpCachedClient(RedisConnectrionstring);
                return await client.GetContentAsync(url);
            }
            else
            {
                HttpRequester.HttpClient client = new HttpRequester.HttpClient();
                return await client.GetContentAsync(url);
            }
        }
    }
}