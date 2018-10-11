using HttpRequester;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
        //public bool ExecuteTidy { get; set; }
        public EnumHttpProvider HttpProvider { get; set; }
        public HttpRequester.Requester client;
        public HttpRequester.CachedRequester cachedRequester;

        public DownloaderMiddleware()
        {
            this.HttpProvider = GlobalSettings.HttpProvider ?? HttpRequester.EnumHttpProvider.HttpClient;
            this.UseRedisCache = GlobalSettings.UseRedisCache ?? false;
            this.RedisConnectrionstring = GlobalSettings.RedisConnectionString ?? null;
        }

        public async Task<string> RunAsync(string url)
        {
            string content = string.Empty;
            if (UseRedisCache)
            {
                cachedRequester = cachedRequester ?? new HttpRequester.CachedRequester(RedisConnectrionstring, HttpProvider, Duration);
                cachedRequester.DefaultHeaders = this.DefaultHeaders ?? new Dictionary<string, string>(); try
                {
                    content = await cachedRequester.GetContentAsync(url);
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                {
                    return string.Empty;
                }
            }
            else
            {
                try
                {
                    client = client ?? new HttpRequester.Requester(HttpProvider);
                    client.DefaultHeaders = DefaultHeaders;
                    client.Cookies = this.Cookies;
                    content = await client.GetContentAsync(url);
                    Cookies = client.Cookies;
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                {
                    return string.Empty;
                }
            }

            /*
            if (ExecuteTidy)
            {
                return content;
            }
            */
            return content;
        }
    }
}