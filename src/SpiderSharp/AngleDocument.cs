using System;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace SpiderSharp
{
    public static class AngleDocument
    {
        public static IHtmlDocument TryParse(string content)
        {
            try
            {
                var parser = new HtmlParser();
                return parser.ParseDocument(content);
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}

