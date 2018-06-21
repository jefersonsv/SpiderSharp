using Newtonsoft.Json.Linq;
using Slugify;
using System.Text.RegularExpressions;

namespace SpiderSharp
{
    public static class Extractors
    {
        public static int? IntOrNull(string val)
        {
            int num;
            if (int.TryParse(val, out num))
            {
                return num;
            }

            return null;
        }

        public static int IntOrOne(string val)
        {
            int num;
            if (int.TryParse(val, out num))
            {
                return num;
            }

            return 1;
        }

        public static JToken JsonList(object obj)
        {
            return JToken.FromObject(obj);
        }

        public static string RegexFirstGroup(string text, string pattern)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            Regex regex = new Regex(pattern);
            if (regex.IsMatch(text))
                return regex.Match(text).Value;
            else
                return null;
        }

        /// <summary>
        /// https://github.com/ctolkien/Slugify
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Slug(string text)
        {
            SlugHelper helper = new SlugHelper();
            return helper.GenerateSlug(text);
        }
    }
}