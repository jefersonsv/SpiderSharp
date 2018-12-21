using HttpRequester;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public class DownloaderMiddleware
    {
        public void ForceCookies(string cookies)
        {
            this.client.ForceCookies = cookies;
        }

        //public string PrefixCache { get; set; }

        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();

        public string HttpMethod { get; set; }
        public string HttpBody { get; set; }
        public string RedisConnectrionstring { get; set; }
        public bool UseRedisCache { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Cookies { get; set; }
        //public bool ExecuteTidy { get; set; }
        public EnumHttpProvider HttpProvider { get; set; }
        public HttpRequester.Requester client;
        public HttpRequester.CachedRequester cachedRequester;

        public int TotalRequestOnline { get; private set; }
        public int TotalRequestCached { get; private set; }

        public DownloaderMiddleware(HttpRequester.EnumHttpProvider provider)
        {
            this.HttpProvider = provider;
            this.client = client ?? new HttpRequester.Requester(this.HttpProvider);
            this.UseRedisCache = GlobalSettings.UseRedisCache ?? false;
            this.RedisConnectrionstring = GlobalSettings.RedisConnectionString ?? null;
        }

        public async Task SimplePostAsync(string url)
        {
            Log.Information($"SIMPLE POST: {url}");
            client.DefaultHeaders = DefaultHeaders;
            //client.Cookies = this.Cookies;
            await client.PostContentAsync(url, new System.Collections.Specialized.NameValueCollection());
            Cookies = client.Cookies;
        }

        public async Task<string> SimpleGetAsync(string url)
        {
            client.DefaultHeaders = DefaultHeaders;
            //client.Cookies = this.Cookies;
            var content = await client.GetContentAsync(url);
            Cookies = client.Cookies;
            return content;
        }

        public async Task<string> RunAsync(string url)
        {
            string content = string.Empty;
            if (UseRedisCache)
            {
                cachedRequester = cachedRequester ?? new HttpRequester.CachedRequester(this.client, RedisConnectrionstring, Duration);
                cachedRequester.DefaultHeaders = this.DefaultHeaders ?? new Dictionary<string, string>();

                //if (!string.IsNullOrEmpty(Cookies))
                //{
                //    cachedRequester.DefaultHeaders["Cookie"] = this.Cookies;
                //}

                try
                {
                    var rc = await cachedRequester.GetContentAsync(url);
                    if (rc.UsedCache)
                        TotalRequestCached++;
                    else
                        TotalRequestOnline++;

                    content = rc.Content;
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
                    client.HttpBody = this.HttpBody;
                    client.HttpMethod = this.HttpMethod;
                    client.DefaultHeaders = DefaultHeaders;
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