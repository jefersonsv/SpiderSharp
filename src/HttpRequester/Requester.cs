using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace HttpRequester
{
    public class Requester
    {
        public string Cookies { get; private set; }
        public bool UseCache { get; set; }
        public string RedisConnectionString { get; set; }
        public EnumHttpProvider HttpProvider { get; private set; }
        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();

        private readonly System.Net.Http.HttpClient httpClient;
        private readonly BetterWebClient betterWebClient;
        private readonly CookieWebClient cookieWebClient;

        public Requester(EnumHttpProvider httpProvider)
        {
            this.HttpProvider = httpProvider;
            switch (HttpProvider)
            {
                case EnumHttpProvider.HttpClient:
                    // http://www.talkingdotnet.com/3-ways-to-use-httpclientfactory-in-asp-net-core-2-1/?utm_source=csharpdigest&utm_medium=email&utm_campaign=featured
                    httpClient = new System.Net.Http.HttpClient();
                    break;

                case EnumHttpProvider.BetterWebClient:
                    betterWebClient = new BetterWebClient();
                    break;

                case EnumHttpProvider.CookieWebClient:
                    cookieWebClient = new CookieWebClient();
                    break;
            }
        }

        public async Task<string> GetContentAsync(string url)
        {
            PublishHeaders();
            switch (HttpProvider)
            {
                case EnumHttpProvider.HttpClient:
                    return await httpClient.GetStringAsync(url);

                case EnumHttpProvider.BetterWebClient:

                    byte[] data = null;

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
                    Cookies = betterWebClient.CookieContainer.GetCookieHeader(new Uri(url));
                    return Encoding.Default.GetString(data);

                case EnumHttpProvider.CookieWebClient:

                    byte[] data2 = null;

                    cookieWebClient.DownloadDataCompleted +=
                    delegate (object sender, DownloadDataCompletedEventArgs e)
                    {
                        data2 = e.Result;
                    };

                    cookieWebClient.DownloadDataAsync(new Uri(url));
                    while (cookieWebClient.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    Cookies = cookieWebClient.CookieContainer.GetCookieHeader(new Uri(url));
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
            PublishHeaders();
            switch (HttpProvider)
            {
                case EnumHttpProvider.HttpClient:
                    return await this.httpClient.PostAsync(url, postData).Result.Content.ReadAsStringAsync();

                case EnumHttpProvider.BetterWebClient:

                    byte[] data = null;

                    betterWebClient.DownloadDataCompleted +=
                    delegate (object sender, DownloadDataCompletedEventArgs e)
                    {
                        data = e.Result;
                    };

                    betterWebClient.UploadDataAsync(new Uri(url), postData.ReadAsByteArrayAsync().Result);
                    while (betterWebClient.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    Cookies = betterWebClient.CookieContainer.GetCookieHeader(new Uri(url));
                    return Encoding.Default.GetString(data);

                case EnumHttpProvider.CookieWebClient:

                    byte[] data2 = null;

                    cookieWebClient.DownloadDataCompleted +=
                    delegate (object sender, DownloadDataCompletedEventArgs e)
                    {
                        data2 = e.Result;
                    };

                    cookieWebClient.UploadDataAsync(new Uri(url), postData.ReadAsByteArrayAsync().Result);
                    while (cookieWebClient.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    Cookies = cookieWebClient.CookieContainer.GetCookieHeader(new Uri(url));
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
            PublishHeaders();
            switch (HttpProvider)
            {
                case EnumHttpProvider.HttpClient:

                    throw new NotImplementedException();
                    //var res = await httpClient.PostAsync(url, "");
                    //return await res.Content.ReadAsStringAsync();

                case EnumHttpProvider.BetterWebClient:

                    byte[] data = null;

                    betterWebClient.DownloadDataCompleted +=
                    delegate (object sender, DownloadDataCompletedEventArgs e)
                    {
                        data = e.Result;
                    };

                    betterWebClient.UploadValuesAsync(new Uri(url), "POST", postData );
                    while (betterWebClient.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    Cookies = betterWebClient.CookieContainer.GetCookieHeader(new Uri(url));
                    return Encoding.Default.GetString(data);

                case EnumHttpProvider.CookieWebClient:

                    byte[] data2 = null;

                    cookieWebClient.DownloadDataCompleted +=
                    delegate (object sender, DownloadDataCompletedEventArgs e)
                    {
                        data2 = e.Result;
                    };

                    cookieWebClient.UploadValuesAsync(new Uri(url), "POST", postData);
                    while (cookieWebClient.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    Cookies = cookieWebClient.CookieContainer.GetCookieHeader(new Uri(url));
                    return Encoding.Default.GetString(data2);
            }

            throw new NotImplementedException();
        }

    }
}
