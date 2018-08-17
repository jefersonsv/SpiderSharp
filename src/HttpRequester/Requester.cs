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

namespace HttpRequester
{
    public class Requester
    {
        public string Cookies { get; set; }
        public bool UseCache { get; set; }
        public string RedisConnectionString { get; set; }
        public EnumHttpProvider HttpProvider { get; private set; }
        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();

        /// <summary>
        /// http://www.talkingdotnet.com/3-ways-to-use-httpclientfactory-in-asp-net-core-2-1/?utm_source=csharpdigest&utm_medium=email&utm_campaign=featured
        /// </summary>
        public readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
        public readonly BetterWebClient betterWebClient = new BetterWebClient();
        public readonly CookieWebClient cookieWebClient = new CookieWebClient();
        public readonly WebClient webClient = new WebClient();
        public readonly IBrowsingContext angleSharpClient = null;
        public readonly ChromeHeadlessClient chromeHeadlessClient = new ChromeHeadlessClient();
        public readonly ChromeHeadlessPersistentClient chromeHeadlessPersistentClient = new ChromeHeadlessPersistentClient();

        public Requester(EnumHttpProvider httpProvider)
        {
            this.HttpProvider = httpProvider;

            // Anglesharp
            var requester = new AngleSharp.Network.Default.HttpRequester();

            if (!DefaultHeaders.ContainsKey("User-Agent"))
                requester.Headers["User-Agent"] = this.spiderSharpUserAgent;

            var configuration = Configuration.Default.WithDefaultLoader(loader =>
                {
                    loader.IsNavigationEnabled = true;
                    loader.IsResourceLoadingEnabled = false;
                },
                requesters: new[] { requester }
            );

            this.angleSharpClient = AngleSharp.BrowsingContext.New(configuration);
        }

        public async Task<string> GetContentAsync(string url)
        {
            var uri = new Uri(url);
            PublishHeaders();
            switch (HttpProvider)
            {
                case EnumHttpProvider.HttpClient:
                    return await httpClient.GetStringAsync(url);

                case EnumHttpProvider.WebClient:
                    return Encoding.Default.GetString(await webClient.DownloadDataTaskAsync(uri));

                case EnumHttpProvider.AngleSharp:

                    var browse = await angleSharpClient.OpenAsync(url);
                    return browse.Source.Text;

                case EnumHttpProvider.BetterWebClient:

                    var data = await betterWebClient.DownloadDataTaskAsync(uri);
                    Cookies = betterWebClient.CookieContainer.GetCookieHeader(uri);
                    return Encoding.Default.GetString(data);

                case EnumHttpProvider.CookieWebClient:

                    var data2 = await cookieWebClient.DownloadDataTaskAsync(uri);
                    Cookies = cookieWebClient.CookieContainer.GetCookieHeader(uri);
                    return Encoding.Default.GetString(data2);

                case EnumHttpProvider.ChromeHeadless:
                    return await chromeHeadlessClient.GetContentAsync(uri.ToString());

                case EnumHttpProvider.ChromeHeadlessPersistent:
                    return await chromeHeadlessPersistentClient.GoAndGetContentAsync(uri.ToString());
            }

            throw new NotImplementedException();
        }

        string spiderSharpUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:61.0) Gecko/20100101 Firefox/61.0";

        void PublishHeaders()
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
                        this.httpClient.DefaultRequestHeaders.Add("User-Agent", this.spiderSharpUserAgent);

                    if (!DefaultHeaders.ContainsKey("Cookie"))
                        this.httpClient.DefaultRequestHeaders.Add("Cookie", this.Cookies);

                break;


                case EnumHttpProvider.BetterWebClient:
                    foreach (var item in DefaultHeaders)
                    {
                        this.betterWebClient.Headers.Add(item.Key, item.Value);
                    }

                    if (!DefaultHeaders.ContainsKey("User-Agent"))
                        this.betterWebClient.Headers.Add("User-Agent", this.spiderSharpUserAgent);

                    if (!DefaultHeaders.ContainsKey("Cookie"))
                        this.betterWebClient.Headers.Add("Cookie", this.Cookies);

                    break;

                case EnumHttpProvider.CookieWebClient:
                    foreach (var item in DefaultHeaders)
                    {
                        this.cookieWebClient.Headers.Add(item.Key, item.Value);
                    }

                    if (!DefaultHeaders.ContainsKey("User-Agent"))
                        this.cookieWebClient.Headers.Add("User-Agent", this.spiderSharpUserAgent);

                    if (!DefaultHeaders.ContainsKey("Cookie"))
                        this.cookieWebClient.Headers.Add("Cookie", this.Cookies);

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
            PublishHeaders();
            switch (HttpProvider)
            {
                case EnumHttpProvider.HttpClient:
                    return await this.httpClient.PostAsync(url, postData).Result.Content.ReadAsStringAsync();

                case EnumHttpProvider.BetterWebClient:

                    var data = await betterWebClient.UploadDataTaskAsync(uri, postData.ReadAsByteArrayAsync().Result);
                    Cookies = betterWebClient.CookieContainer.GetCookieHeader(uri);
                    return Encoding.Default.GetString(data);

                case EnumHttpProvider.CookieWebClient:

                    var data2 = await cookieWebClient.UploadDataTaskAsync(uri, postData.ReadAsByteArrayAsync().Result);
                    Cookies = cookieWebClient.CookieContainer.GetCookieHeader(uri);
                    return Encoding.Default.GetString(data2);
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
            PublishHeaders();
            switch (HttpProvider)
            {
                case EnumHttpProvider.BetterWebClient:
                    var data = await betterWebClient.UploadValuesTaskAsync(uri, "POST", postData );
                    Cookies = betterWebClient.CookieContainer.GetCookieHeader(uri);
                    return Encoding.Default.GetString(data);

                case EnumHttpProvider.CookieWebClient:
                    var data2 = await cookieWebClient.UploadValuesTaskAsync(uri, "POST", postData);
                    Cookies = cookieWebClient.CookieContainer.GetCookieHeader(uri);
                    return Encoding.Default.GetString(data2);
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