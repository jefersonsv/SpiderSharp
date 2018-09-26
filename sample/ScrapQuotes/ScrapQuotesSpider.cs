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

            var idx = 0;
            foreach (var item in quotes)
            {
                try
                {
                    ct.Data.text = item.GetInnerText("span.text");

                    if (idx == 1)
                        throw new Exception("error occour");
                    ct.Data.author = item.GetInnerText("small.author");
                    ct.Data.tags = item.SelectInnerText("div.tags > a.tag");
                }
                catch (Exception ex)
                {
                    ct.Error = ex;
                }

                idx++;
                yield return ct;
            }
        }

        protected override void SuccessPipeline(SpiderContext context)
        {
            context.RunPrintToConsolePipeline();
        }

        protected override void ErrorPipeline(SpiderContext context)
        {
            context.RunEmbedException();
            context.RunPrintToConsolePipeline();
        }
    }
}
