using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SpiderSharp.Helpers
{
    public static class Html
    {
        public static JToken GetTags(string title, JArray categories)
        {
            var tags1 = title.ToUpper().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var tags2 = categories.Select(a => a.ToString()).SelectMany(s => s.ToUpper().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            var tags3 = System.Linq.Enumerable.Concat(tags1, tags2);
            return Extractors.JsonList(tags3.Distinct().Where(w => w.All(a => Char.IsLetter(a))).OrderBy(o => o));
        }

        public static Nodes TryParse(string content)
        {
            try
            {
                return new Nodes(content);
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}
