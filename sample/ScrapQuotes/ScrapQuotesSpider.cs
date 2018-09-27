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

        protected override IEnumerable<SpiderContext> OnRun()
        {
            var quotes = this.node.SelectNodes("div.quote");

            foreach (var item in quotes)
            {
                yield return this.Fetch(() => {
                    ct.Data.text = item.GetInnerText("span.text");
                    ct.Data.author = item.GetInnerText("small.author");
                    ct.Data.tags = item.SelectInnerText("div.tags > a.tag");
                });
            }
        }

        protected override void SuccessPipeline(SpiderContext context)
        {
            context.RunPrintToConsolePipeline();
        }

        protected override void ErrorPipeline(SpiderContext context)
        {
            context.RunEmbedMetadata();
            context.RunPrintToConsolePipeline();
        }
    }
}
