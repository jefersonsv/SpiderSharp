using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiderSharp.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

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


        public string GetAttribute(string attribute)
        {
            return doc?.Attributes[attribute]?.Value;
        }

        public string GetAttribute(string cssSelector, string attribute)
        {
            var node = doc.QuerySelector(cssSelector);
            return node?.Attributes[attribute]?.Value;
        }

        public string GetAttributeValue(string attribute)
        {
            return this.doc.Attributes[attribute]?.Value;
        }

        public string GetAttributeValue(string cssSelector, string attribute)
        {
            var node = doc.QuerySelector(cssSelector);
            return node?.Attributes[attribute]?.Value;
        }

        public string GetClass(string cssSelector)
        {
            return this.GetAttribute(cssSelector, "class");
        }

        public string GetHref()
        {
            return this.doc.Attributes["href"].Value;
        }

        public bool HasHref()
        {
            return this.doc.Attributes.Any(w => w.Name == "href");
        }

        public bool HasClass()
        {
            return this.doc.Attributes.Any(w => w.Name == "class");
        }

        public string GetHref(string cssSelector)
        {
            return GetAttribute(cssSelector, "href");
        }

        public string GetTitle(string cssSelector)
        {
            return GetAttribute(cssSelector, "title");
        }

        public Nodes GetParentNode()
        {
            return new Nodes(this.doc.ParentNode);
        }

        public string GetSrc(string cssSelector)
        {
            return GetAttribute(cssSelector, "src");
        }

        public string GetInnerText(string cssSelector)
        {
            var node = doc.QuerySelector(cssSelector);
            return System.Web.HttpUtility.HtmlDecode(node?.InnerText);
        }

        public string GetFirstChildInnerText(string cssSelector)
        {
            var node = doc.QuerySelector(cssSelector);
            return System.Web.HttpUtility.HtmlDecode(node?.FirstChild.InnerText);
        }

        public string GetInnerText()
        {
            return System.Web.HttpUtility.HtmlDecode(doc?.InnerText);
        }

        public string GetInnerHtml()
        {
            return System.Web.HttpUtility.HtmlDecode(doc?.InnerHtml);
        }

        public JObject GetJson()
        {
            JObject obj = new JObject();
            obj.Add(new JProperty(doc?.Name, HtmlNodeToJObject(doc)));

            return obj;
        }


        static JObject HtmlNodeToJObject(HtmlNode node)
        {
            JObject obj = new JObject();

            node?.ChildNodes.ToList().ForEach(a =>
            {
                // add childs
                if (a.NodeType == HtmlNodeType.Element)
                {
                    var no = HtmlNodeToJObject(a);
                    if (no.Children().Count() > 0)
                    {
                        // verifiy if is unique or array
                        if (node?.ChildNodes.Where(w => w.Name == a.Name).Count() > 1)
                        {
                            // is array

                            // verify if is first
                            if (obj.ContainsKey(a.Name))
                            {
                                // second, third
                                JArray arr = obj[a.Name] as JArray;
                                arr.Add(no);
                            }
                            else
                            {
                                // first
                                obj.Add(new JProperty(a.Name, new JArray(no)));
                            }
                        }
                        else
                        {
                            // is unique
                            obj.Add(new JProperty(a.Name, no));
                        }
                    }
                }
                else if (a.NodeType == HtmlNodeType.Text)
                {
                    if (!string.IsNullOrWhiteSpace(a.InnerText))
                    {
                        var ct = System.Web.HttpUtility.HtmlDecode(a.InnerText.Trim());
                        if (!obj.ContainsKey(a.Name))
                        {
                            obj.Add(new JProperty(a.Name, ct));
                        }
                        else
                        {
                            obj.Add(new JProperty(a.Name + "_" + obj.Count, ct));
                        }
                    }
                }
            });

            // add attributes
            node?.Attributes.ToList().ForEach(a => {

                if (!string.IsNullOrWhiteSpace(a.Value))
                {
                    string ct = System.Web.HttpUtility.HtmlDecode(a.Value.ToString());
                    // verifiy if is unique or array
                    if (node?.Attributes.Where(w => w.Name == a.Name).Count() > 1)
                    {
                        // is array

                        // verify if is first
                        if (obj.ContainsKey($"@{a.Name}"))
                        {
                            // second, third
                            JArray arr = obj[$"@{a.Name}"] as JArray;
                            arr.Add(ct);
                        }
                        else
                        {
                            // first
                            obj.Add(new JProperty($"@{a.Name}", new JArray(ct)));
                        }
                    }
                    else
                    {
                        // is single
                        obj.Add(new JProperty($"@{a.Name}", ct));
                    }
                }
            });

            return obj;
        }


        public string GetOuterHtml()
        {
            return System.Web.HttpUtility.HtmlDecode(doc?.OuterHtml);
        }

        public string GetHtml()
        {
            return doc.OwnerDocument.DocumentNode.OuterHtml;
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
            return nodes?
                .Where(w => w.Attributes.Contains("href"))
                .Select(s => s.Attributes["href"].Value);
        }

        public IEnumerable<string> SelectInnerText(string cssSelector)
        {
            var nodes = doc.QuerySelectorAll(cssSelector);
            return nodes?.Select(s => System.Web.HttpUtility.HtmlDecode(s.InnerText));
        }

        public IEnumerable<Nodes> SelectNodes(string cssSelector)
        {
            return doc.QuerySelectorAll(cssSelector)
                .Select(s => new Nodes(s));
        }

        public IEnumerable<Nodes> XSelectNodes(string xpath)
        {
            return doc.SelectNodes(xpath)
                .Select(s => new Nodes(s));
        }

        public Nodes GetNode(string cssSelector)
        {
            var node = doc.QuerySelector(cssSelector);
            return new Nodes(node);
        }
    }
}