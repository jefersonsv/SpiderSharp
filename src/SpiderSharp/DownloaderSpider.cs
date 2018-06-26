using HttpRequester;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public class DownloaderMiddleware
    {
        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();
        public string RedisConnectrionstring { get; set; }
        public bool UseRedisCache { get; set; }
        public string Cookies { get; private set; }

        public EnumHttpProvider HttpProvider { get; set; }

        public async Task<string> RunAsync(string url)
        {
            if (UseRedisCache)
            {
                HttpRequester.CachedRequester client = new HttpRequester.CachedRequester(RedisConnectrionstring, HttpProvider);
                return await client.GetContentAsync(url);
            }
            else
            {
                HttpRequester.Requester client = new HttpRequester.Requester(HttpProvider);
                client.DefaultHeaders = DefaultHeaders;
                var resp = await client.GetContentAsync(url);
                Cookies = client.Cookies;
                return resp;
            }
        }
    }
}