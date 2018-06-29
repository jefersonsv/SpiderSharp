using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using AngleSharp;
using System.IO;

namespace HttpRequester
{
    public class Requester
    {
        public string Cookies { get; private set; }
        public bool UseCache { get; set; }
        public string RedisConnectionString { get; set; }
        public EnumHttpProvider HttpProvider { get; private set; }
        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();

        /// <summary>
        /// http://www.talkingdotnet.com/3-ways-to-use-httpclientfactory-in-asp-net-core-2-1/?utm_source=csharpdigest&utm_medium=email&utm_campaign=featured
        /// </summary>
        private readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
        private readonly BetterWebClient betterWebClient = new BetterWebClient();
        private readonly CookieWebClient cookieWebClient = new CookieWebClient();
        private readonly WebClient webClient = new WebClient();
        private readonly IBrowsingContext angleSharpClient = null;

        public Requester(EnumHttpProvider httpProvider)
        {
            this.HttpProvider = httpProvider;

            // Anglesharp
            var requester = new AngleSharp.Network.Default.HttpRequester();
            requester.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
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

                    //using (var memoryStream = new MemoryStream())
                    //using (var tw = new StreamWriter(memoryStream))
                    //{
                    //    var formatter = new AngleSharp.Xml.XmlMarkupFormatter();
                    //    var d1 = await angleSharpClient.OpenAsync(url);
                    //    d1.ToHtml(tw, formatter);
                    //    tw.Flush();
                    //    memoryStream.Flush();

                    //    return Encoding.Default.GetString(memoryStream.ToArray());
                    //}

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
            }

            throw new NotImplementedException();
        }

        void PublishHeaders()
        {
            switch (this.HttpProvider)
            {
                case EnumHttpProvider.HttpClient:
                    foreach (var item in DefaultHeaders)
                    {
                        this.httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                    break;


                case EnumHttpProvider.BetterWebClient:
                    foreach (var item in DefaultHeaders)
                    {
                        this.betterWebClient.Headers.Add(item.Key, item.Value);
                    }
                    break;

                case EnumHttpProvider.CookieWebClient:
                    foreach (var item in DefaultHeaders)
                    {
                        this.cookieWebClient.Headers.Add(item.Key, item.Value);
                    }
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