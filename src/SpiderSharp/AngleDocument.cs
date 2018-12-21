using System;
using System.Collections.Generic;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;

namespace SpiderSharp
{
    public static class AngleDocument
    {
        public static IHtmlDocument TryParse(string content)
        {
            try
            {
                var parser = new HtmlParser();
                return parser.Parse(content);
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}

