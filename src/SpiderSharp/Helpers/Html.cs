using System;
using System.Text.RegularExpressions;

namespace SpiderSharp.Helpers
{
    public static class Html
    {
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

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="https://stackoverflow.com/questions/18153998/how-do-i-remove-all-html-tags-from-a-string-without-knowing-which-tags-are-in-it"/>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}