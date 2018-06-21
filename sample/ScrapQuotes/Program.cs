using System;
using SpiderSharp;

namespace ScrapQuotes
{
    class Program
    {
        static void Main(string[] args)
        {
            ScrapQuotesSpider spider = new ScrapQuotesSpider();
            spider.AddPrintToConsolePipeline();
            spider.Run();
        }
    }
}
