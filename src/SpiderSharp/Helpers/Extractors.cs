using Newtonsoft.Json.Linq;

using Slugify;

using System.Text.RegularExpressions;

namespace SpiderSharp.Helpers
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

        public static int? NumbersOrNull(string value)
        {
            if (value == null)
                return null;

            var num = Regex.Match(value, @"\d+").Value;
            return IntOrNull(num);
        }

        public static string NumbericOrNull(string value)
        {
            if (value == null)
                return null;

            return Regex.Match(value, @"\d+").Value;
        }

        public static JToken JsonList(object obj)
        {
            return JToken.FromObject(obj);
        }

        public static string RegexUrl(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var match = RegexLibrary.Url.Match(text);
            if (match.Success)
                return match.Value;

            return null;
        }

        public static string RegexFirstGroup(string text, string pattern)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            Regex regex = new Regex(pattern);
            if (regex.IsMatch(text))
                return regex.Match(text).Groups[0].Value;
            else
                return null;
        }

        public static string RegexLastGroup(string text, string pattern)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            Regex regex = new Regex(pattern);
            if (regex.IsMatch(text))
            {
                var match = regex.Match(text);
                return match.Groups[match.Groups.Count - 1].Value;
            }
            else
                return null;
        }

        public static string RegexGroup(string text, string pattern, string group)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            Regex regex = new Regex(pattern);
            if (regex.IsMatch(text))
            {
                var match = regex.Match(text);
                return match.Groups[group].Value;
            }
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