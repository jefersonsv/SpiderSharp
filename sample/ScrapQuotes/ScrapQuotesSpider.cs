using System;
using System.Collections.Generic;
using System.Text;
using SpiderSharp;
using System.Dynamic;

namespace ScrapQuotes
{
    public class ScrapQuotesSpider : SpiderEngine, ISpiderEngine
    {
        public ScrapQuotesSpider()
        {
            this.SetUrl("http://quotes.toscrape.com");
        }

        protected override string FollowPage()
        {
            return $"http://quotes.toscrape.com{this.node.GetHref("ul > li > a")}";
        }

        protected override IEnumerable<dynamic> OnRun()
        {
            dynamic json = new ExpandoObject();

            var quotes = this.node.SelectNodes("div.quote");
            foreach (var item in quotes)
            {
                json.text = item.GetInnerText("span.text");
                json.author = item.GetInnerText("small.author");
                json.tags = item.SelectInnerText("div.tags > a.tag");

                yield return json;
            }
        }
    }
}
