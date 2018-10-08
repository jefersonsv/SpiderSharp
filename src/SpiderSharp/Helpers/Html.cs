using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SpiderSharp.Helpers
{
    public static class Html
    {
        static string[] TAGS_REMOVE = { "HOME", "FOR", "WITH" };

        public static JToken GetTags(string title, string[] categoriesArr)
        {
            var titleTags = title.ToUpper().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var categoryTags = categoriesArr.SelectMany(s => s.ToUpper().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            var joined = System.Linq.Enumerable.Concat(titleTags, categoryTags);
            var tags = joined.Distinct().Where(w => w.All(a => Char.IsLetter(a))).OrderBy(o => o).ToList();
            tags.RemoveAll(w => TAGS_REMOVE.Contains(w) || w.Length < 3);

            return Extractors.JsonList(tags);
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
