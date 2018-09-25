using HttpRequester;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public class DownloaderMiddleware
    {
        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();
        public string RedisConnectrionstring { get; set; }
        public bool UseRedisCache { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Cookies { get; set; }

        public EnumHttpProvider HttpProvider { get; set; }

        public HttpRequester.Requester client;
        public HttpRequester.CachedRequester cachedRequester;

        public async Task<string> RunAsync(string url)
        {
            client = client ?? new HttpRequester.Requester(HttpProvider);
            if (UseRedisCache)
            {
                cachedRequester = cachedRequester ?? new HttpRequester.CachedRequester(RedisConnectrionstring, HttpProvider, Duration);
                var content = await client.GetContentAsync(url);
                return content;
            }
            else
            {
                client.DefaultHeaders = DefaultHeaders;
                client.Cookies = this.Cookies;
                var resp = client.GetContentAsync(url).Result;
                Cookies = client.Cookies;
                return resp;
            }
        }
    }
}