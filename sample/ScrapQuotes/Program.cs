using System;
using SpiderSharp;

namespace ScrapQuotes
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpRequester.Requester client = new HttpRequester.Requester(HttpRequester.EnumHttpProvider.AngleSharp);
            var uol = client.GetContentAsync("https://www.uol.com.br").Result;

            ScrapQuotesSpider spider = new ScrapQuotesSpider();
            spider.RunAsync().Wait();
        }
    }
}
