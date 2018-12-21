using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using AngleSharp;
using System.IO;
using System.Diagnostics;
using Serilog;

namespace HttpRequester
{
    public class Requester
    {
        public string ForceCookies { private get; set; }
        public string Cookies { get; private set; }
        public bool UseCache { get; set; }
        public string RedisConnectionString { get; set; }
        public EnumHttpProvider HttpProvider { get; private set; }
        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();

        public string HttpMethod { get; set; }
        public string HttpBody { get; set; }

        /// <summary>
        /// http://www.talkingdotnet.com/3-ways-to-use-httpclientfactory-in-asp-net-core-2-1/?utm_source=csharpdigest&utm_medium=email&utm_campaign=featured
        /// </summary>
        public readonly System.Net.Http.HttpClient httpClient = null;
        public readonly BetterWebClient betterWebClient = null;
        public readonly WebClient webClient = null;
        //public readonly IBrowsingContext angleSharpClient = null;
        public IBrowsingContext angleSharpClient = null;
        public readonly ChromeHeadlessClient chromeHeadlessClient = null;
        public readonly ChromeHeadlessPersistentClient chromeHeadlessPersistentClient = null;

        public Requester(EnumHttpProvider httpProvider)
        {
            this.HttpProvider = httpProvider;

            if (httpProvider == EnumHttpProvider.AngleSharp)
            {
                //// Anglesharp
                //var requester = new AngleSharp.Network.Default.HttpRequester();

                //if (!DefaultHeaders.ContainsKey("User-Agent"))
                //    requester.Headers["User-Agent"] = this.SpiderSharpUserAgent;

                //var configuration = Configuration.Default.WithDefaultLoader(loader =>
                //{
                //    loader.IsNavigationEnabled = true;
                //    loader.IsResourceLoadingEnabled = false;
                //},
                //    requesters: new[] { requester }
                //);

                this.angleSharpClient = AngleSharp.BrowsingContext.New();
            }
            else if (httpProvider == EnumHttpProvider.BetterWebClient)
            {
                this.betterWebClient = new BetterWebClient();
            }
            else if (httpProvider == EnumHttpProvider.ChromeHeadless)
            {
                this.chromeHeadlessClient = new ChromeHeadlessClient();
            }
            else if (httpProvider == EnumHttpProvider.ChromeHeadlessPersistent)
            {
                this.chromeHeadlessPersistentClient = new ChromeHeadlessPersistentClient();
            }
            else if (httpProvider == EnumHttpProvider.HttpClient)
            {
                this.httpClient = new System.Net.Http.HttpClient();
            }
            else if (httpProvider == EnumHttpProvider.WebClient)
            {
                this.webClient = new WebClient();
            }
            else
            {
                throw new ArgumentNullException("httpProvider");
            }
        }

        public async Task<string> GetContentAsync(string url)
        {
            var uri = new Uri(url);
            PublishHeaders(url);
            switch (HttpProvider)
            {
                case EnumHttpProvider.HttpClient:

                    var ret = await httpClient.GetAsync(url);
                    return await ret.Content.ReadAsStringAsync();
                    //if (ret.StatusCode == HttpStatusCode.OK)
                    //{
                        
                    //}
                    //else
                    //{
                    //    throw new Exception(ret.ReasonPhrase);
                    //}

                case EnumHttpProvider.WebClient:
                    return Encoding.Default.GetString(await webClient.DownloadDataTaskAsync(uri));

                case EnumHttpProvider.AngleSharp:

                    var browse = await angleSharpClient.OpenAsync(url);
                    return browse.Source.Text;

                case EnumHttpProvider.BetterWebClient:

                    if (this.HttpMethod == "POST")
                    {
                        var msg = Encoding.UTF8.GetBytes(this.HttpBody);
                        var result = await betterWebClient.UploadDataTaskAsync(uri, "POST", msg);

                        Cookies = betterWebClient.CookieContainer.GetCookieHeader(uri);

                        return Encoding.Default.GetString(result);
                    }

                    var data = await betterWebClient.DownloadDataTaskAsync(uri);
                    Cookies = betterWebClient.CookieContainer.GetCookieHeader(uri);
                    
                    //var debug = System.IO.Path.GetTempFileName() + ".html";
                    //System.IO.File.WriteAllText(debug, System.Text.Encoding.Default.GetString(data));
                    //Log.Information($"Debug: {debug}");

                    return Encoding.Default.GetString(data);

                case EnumHttpProvider.ChromeHeadless:
                    return await chromeHeadlessClient.GetContentAsync(uri.ToString());

                case EnumHttpProvider.ChromeHeadlessPersistent:
                    return await chromeHeadlessPersistentClient.GoAndGetContentAsync(uri.ToString());
            }

            throw new NotImplementedException();
        }

        public string SpiderSharpUserAgent = new UserAgentSwitcher().GetRandom();
        public string SpiderSharpAcceptLanguage = "en-US;q=0.8,en;q=0.7";

        void PublishHeaders(string url)
        {
            // Check for
            //if (!DefaultHeaders.ContainsKey("User-Agent"))
                //Mozilla / 5.0(Windows NT 10.0; Win64; x64; rv: 61.0) Gecko / 20100101 Firefox / 61.0

            switch (this.HttpProvider)
            {
                case EnumHttpProvider.HttpClient:
                    foreach (var item in DefaultHeaders)
                    {
                        this.httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }

                    if (!DefaultHeaders.ContainsKey("User-Agent"))
                        this.httpClient.DefaultRequestHeaders.Add("User-Agent", this.SpiderSharpUserAgent);

                    if (!DefaultHeaders.ContainsKey("Accept-Language"))
                        this.httpClient.DefaultRequestHeaders.Add("Accept-Language", this.SpiderSharpAcceptLanguage);

                    if (!string.IsNullOrEmpty(this.ForceCookies))
                    {
                        this.httpClient.DefaultRequestHeaders.Add("Cookie", this.ForceCookies);
                        ForceCookies = string.Empty;
                    }

                    break;

                case EnumHttpProvider.BetterWebClient:
                    foreach (var item in DefaultHeaders)
                    {
                        if (item.Key == "Connection")
                            continue;

                        this.betterWebClient.Headers[item.Key] = item.Value;
                    }

                    if (!DefaultHeaders.ContainsKey("User-Agent"))
                        this.betterWebClient.Headers.Add("User-Agent", this.SpiderSharpUserAgent);

                    if (!DefaultHeaders.ContainsKey("Accept-Language"))
                        this.betterWebClient.Headers.Add("Accept-Language", this.SpiderSharpAcceptLanguage);

                    if (!string.IsNullOrEmpty(this.ForceCookies))
                    {
                        this.betterWebClient.CookieContainer.SetCookies(new Uri(url), this.ForceCookies);
                        //this.betterWebClient.Headers.Add("Cookie", this.ForceCookies);
                        ForceCookies = string.Empty;
                    }

                    // Betterwebclient already keep cookie container
                    //if (!DefaultHeaders.ContainsKey("Cookie"))
                    //this.betterWebClient.Headers.Add("Cookie", this.Cookies);

                    break;

                case EnumHttpProvider.AngleSharp:

                    // Anglesharp
                    var requester = new AngleSharp.Network.Default.HttpRequester();
                    requester.Headers.Clear();

                    foreach (var item in DefaultHeaders)
                    {
                        requester.Headers[item.Key] = item.Value;
                    }

                    if (!DefaultHeaders.ContainsKey("User-Agent"))
                        requester.Headers["User-Agent"] = this.SpiderSharpUserAgent;

                    if (!DefaultHeaders.ContainsKey("Accept-Language"))
                        requester.Headers["Accept-Language"] = this.SpiderSharpAcceptLanguage;

                    if (!string.IsNullOrEmpty(this.ForceCookies))
                    {
                        requester.Headers["Cookie"] = this.ForceCookies;
                        ForceCookies = string.Empty;
                    }

                    var configuration = Configuration.Default.WithDefaultLoader(loader =>
                    {
                        loader.IsNavigationEnabled = true;
                        loader.IsResourceLoadingEnabled = false;
                    },
                        requesters: new[] { requester }
                    );

                    angleSharpClient = AngleSharp.BrowsingContext.New(configuration);
                    break;
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/11145053/cant-find-how-to-use-httpcontent
        /// var stringContent = new FormUrlEncodedContent(new[]
        /// {
        ///     new KeyValuePair<string, string>("email", "jefersonsv@gmail.com"),
        ///     new KeyValuePair<string, string>("password", "***"),
        ///     new KeyValuePair<string, string>("remember", "false"),
        /// });
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public async Task<string> PostContentAsync(string url, HttpContent postData)
        {
            var uri = new Uri(url);
            PublishHeaders(url);
            switch (HttpProvider)
            {
                case EnumHttpProvider.HttpClient:
                    return await this.httpClient.PostAsync(url, postData).Result.Content.ReadAsStringAsync();

                case EnumHttpProvider.BetterWebClient:

                    var data = await betterWebClient.UploadDataTaskAsync(uri, postData.ReadAsByteArrayAsync().Result);
                    Cookies = betterWebClient.CookieContainer.GetCookieHeader(uri);
                    return Encoding.Default.GetString(data);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// var loginData = new System.Collections.Specialized.NameValueCollection
        /// {
        ///     { "email", "jefersonsv@gmail.com" },
        ///     { "password", "****" },
        ///     { "remember", "false" }
        /// };
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public async Task<string> PostContentAsync(string url, System.Collections.Specialized.NameValueCollection postData) 
        {
            var uri = new Uri(url);
            PublishHeaders(url);
            switch (HttpProvider)
            {
                case EnumHttpProvider.HttpClient:

                    List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                    foreach (KeyValuePair<string, string> item in postData)
                    {
                        list.Add(item);
                    }

                    var stringContent = new FormUrlEncodedContent(list);
                    var post = await this.httpClient.PostAsync(url, stringContent);
                    return await post.Content.ReadAsStringAsync();

                case EnumHttpProvider.BetterWebClient:
                    var data = await betterWebClient.UploadValuesTaskAsync(uri, "POST", postData );
                    Cookies = betterWebClient.CookieContainer.GetCookieHeader(uri);
                    return Encoding.Default.GetString(data);
            }

            throw new NotImplementedException();
        }

    }
}

/*
 * byte[] data = null;

                    betterWebClient.DownloadDataCompleted +=
                    delegate (object sender, DownloadDataCompletedEventArgs e)
                    {
                        data = e.Result;
                    };

                    betterWebClient.DownloadDataAsync(new Uri(url));
                    while (betterWebClient.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

*/