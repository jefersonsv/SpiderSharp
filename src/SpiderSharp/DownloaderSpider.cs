using HttpRequester;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public class DownloaderMiddleware
    {
        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();
        //public Dictionary<string, string> DefaultCookies = new Dictionary<string, string>();

        public string RedisConnectrionstring { get; set; }
        public bool UseRedisCache { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Cookies { get; set; }
        public EnumHttpProvider HttpProvider { get; set; }
        public HttpRequester.Requester client;
        public HttpRequester.CachedRequester cachedRequester;

        public async Task<string> RunAsync(string url)
        {
            if (UseRedisCache)
            {
                cachedRequester = cachedRequester ?? new HttpRequester.CachedRequester(RedisConnectrionstring, HttpProvider, Duration);
                cachedRequester.DefaultHeaders = GlobalSettings.DefaultHeaders;
                var content = await cachedRequester.GetContentAsync(url);
                return content;
            }
            else
            {
                try
                {
                    client = client ?? new HttpRequester.Requester(HttpProvider);
                    client.DefaultHeaders = DefaultHeaders;
                    client.Cookies = this.Cookies;
                    var resp = await client.GetContentAsync(url);
                    Cookies = client.Cookies;
                    return resp;
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                {
                    return string.Empty;
                }
            }
        }
    }
}