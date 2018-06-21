using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.Collections.Generic;
using System.Linq;

namespace SpiderSharp
{
    public class Nodes
    {
        private HtmlNode doc;

        public Nodes(string sourceCode)
        {
            var document = new HtmlDocument();
            document.LoadHtml(sourceCode);

            this.doc = document.DocumentNode;
        }

        public Nodes(HtmlNode node)
        {
            this.doc = node;
        }

        public bool Exist(string cssSelector)
        {
            var node = doc.QuerySelector(cssSelector);
            return node != null;
        }

        public string GetAttribute(string cssSelector, string attribute)
        {
            var node = doc.QuerySelector(cssSelector);
            return node?.Attributes[attribute].Value;
        }

        public string GetAttributeValue(string attribute)
        {
            return this.doc.Attributes[attribute].Value;
        }

        public string GetAttributeValue(string cssSelector, string attribute)
        {
            var node = doc.QuerySelector(cssSelector);
            return node.Attributes[attribute].Value;
        }

        public string GetClass(string cssSelector)
        {
            return this.GetAttribute(cssSelector, "class");
        }

        public string GetHref(string cssSelector)
        {
            return GetAttribute(cssSelector, "href");
        }

        public string GetInnerText(string cssSelector)
        {
            var node = doc.QuerySelector(cssSelector);
            return node?.InnerText;
        }

        public IEnumerable<string> GetLinksAll(string cssSelector)
        {
            IList<HtmlNode> nodes = doc.QuerySelectorAll(cssSelector);
            return nodes
                .Select(s => s.GetAttributeValue("href", null))
                .Where(w => w != null);
        }

        public IEnumerable<KeyValuePair<string, string>> GetLinksEnumerable(string cssSelector)
        {
            IList<HtmlNode> nodes = doc.QuerySelectorAll(cssSelector);
            return nodes
                .Where(w => w.GetAttributeValue("href", null) != null)
                .Select(s => new KeyValuePair<string, string>(s.GetAttributeValue("href", null), s.InnerText));
        }

        public IEnumerable<string> SelectAttribute(string cssSelector, string attribute)
        {
            var nodes = doc.QuerySelectorAll(cssSelector);
            return nodes?.Select(s => s.Attributes[attribute].Value);
        }

        public IEnumerable<string> SelectHref(string cssSelector)
        {
            var nodes = doc.QuerySelectorAll(cssSelector);
            return nodes?.Select(s => s.Attributes["href"].Value);
        }

        public IEnumerable<string> SelectInnerText(string cssSelector)
        {
            var nodes = doc.QuerySelectorAll(cssSelector);
            return nodes?.Select(s => s.InnerText);
        }

        public IEnumerable<Nodes> SelectNodes(string cssSelector)
        {
            return doc.QuerySelectorAll(cssSelector)
                .Select(s => new Nodes(s));
        }
    }
}