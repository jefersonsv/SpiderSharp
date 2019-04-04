using HttpRequester;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public class DownloaderMiddleware
    {
        public void ForceCookies(string cookies)
        {
            //this.client.ForceCookies = cookies;
            this.client.SetHeader("Cookie", cookies);
        }

        //public string PrefixCache { get; set; }

        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();

        public string HttpMethod { get; set; }
        public string HttpBody { get; set; }

        public List<KeyValuePair<string, string>> PostData = new List<KeyValuePair<string, string>>();

        public string RedisConnectrionstring { get; set; }

        public string RedisPassword { get; set; }
        public bool UseRedisCache { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Cookies { get; set; }
        //public bool ExecuteTidy { get; set; }
        public EnumHttpProvider HttpProvider { get; set; }
        public HttpRequester.RequesterCached client;
        public HttpRequester.RequesterCached cachedRequester;

        public int TotalRequestOnline { get; private set; }
        public int TotalRequestCached { get; private set; }

        public bool IsFirstMultipleByNumberOnlineRequest(int number)
        {
            return this.TotalRequestOnline % number == 0;
        }

        public DownloaderMiddleware(HttpRequester.EnumHttpProvider provider)
        {
            this.HttpProvider = provider;
            this.client = client ?? new HttpRequester.RequesterCached(this.HttpProvider);

            this.UseRedisCache = GlobalSettings.UseRedisCache ?? false;
            if (this.UseRedisCache)
            {
                this.RedisConnectrionstring = GlobalSettings.RedisConnectionString ?? null;
                this.RedisPassword = GlobalSettings.RedisPassword ?? null;

                var redis = new DataFoundation.Redis.RedisConnection(this.RedisConnectrionstring, this.RedisPassword);
                this.cachedRequester = this.cachedRequester ?? new HttpRequester.RequesterCached(this.HttpProvider, new CacheProvider(redis, this.Duration));
            }
            else
            {
                this.cachedRequester = this.client;
            }
        }

        //public async Task SimplePostAsync(string url)
        //{
        //    Log.Information($"SIMPLE POST: {url}");
        //    //client.DefaultHeaders = DefaultHeaders;
        //    DefaultHeaders.ToList().ForEach(a => client.SetHeader(a.Key, a.Value));
        //    //client.Cookies = this.Cookies;
        //    client.SetHeader("Cookie", this.Cookies);
        //    await client.PostAsync(url, (string) null);
        //    Cookies = client.LastCookie;
        //}

        public async Task<string> RunAsync(string url)
        {
            string content = string.Empty;
        
            try
            {
                string rc = null;
                if (string.IsNullOrEmpty(this.HttpMethod) || this.HttpMethod == "GET")
                {
                    var ct = await cachedRequester.GetAsync(url);
                    if (ct.HasErrors)
                    {
                        throw ct.Exception;
                    }

                        rc = ct.StringContent;

                    if (ct.HasUsedCache)
                        TotalRequestCached++;
                    else
                        TotalRequestOnline++;
                }
                else if(this.HttpMethod == "POST")
                {
                    ResponseContext ct = null;
                    if (!string.IsNullOrEmpty(this.HttpBody))
                    {
                        ct = await cachedRequester.PostAsync(url, this.HttpBody);
                        if (ct.HasErrors)
                        {
                            throw ct.Exception;
                        }
                        rc = ct.StringContent;
                    }
                    else if (PostData.Any())
                    {
                        ct = await cachedRequester.PostAsync(url, this.PostData);
                        if (ct.HasErrors)
                        {
                            throw ct.Exception;
                        }
                        rc = ct.StringContent;
                    }
                    else
                    {
                        ct = await cachedRequester.PostAsync(url, string.Empty);
                        if (ct.HasErrors)
                        {
                            throw ct.Exception;
                        }
                    }

                    if (ct.HasUsedCache)
                        TotalRequestCached++;
                    else
                        TotalRequestOnline++;
                }

                content = rc;
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                return string.Empty;
            }
         
            return content;
        }
    }
}