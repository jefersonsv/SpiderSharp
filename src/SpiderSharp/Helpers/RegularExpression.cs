using System.Text.RegularExpressions;

namespace SpiderSharp.Helpers
{
    public static class RegeLibrary
    {
        /// <summary>
        /// <see cref="// https://stackoverflow.com/questions/19233413/how-to-parse-multiple-cookies-values-from-set-cookie-contains-commas-and-semico"/>
        /// </summary>
        public static readonly Regex Cookies = new Regex(@"(.*?)=(.*?)($|;|,(?! ))", RegexOptions.Singleline);
    }
}