using System.Text.RegularExpressions;

namespace SpiderSharp.Helpers
{
    public static class RegexLibrary
    {
        /// <summary>
        /// 
        /// </summary>
        /// <see cref="https://stackoverflow.com/questions/19233413/how-to-parse-multiple-cookies-values-from-set-cookie-contains-commas-and-semico"/>
        public static readonly Regex Cookies = new Regex(@"(.*?)=(.*?)($|;|,(?! ))", RegexOptions.Singleline);

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="https://stackoverflow.com/questions/6038061/regular-expression-to-find-urls-within-a-string"/>
        public static readonly Regex Url = new Regex(@"(?:(?:https?|ftp):\/\/|\b(?:[a-z\d]+\.))(?:(?:[^\s()<>]+|\((?:[^\s()<>]+|(?:\([^\s()<>]+\)))?\))+(?:\((?:[^\s()<>]+|(?:\(?:[^\s()<>]+\)))?\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))?", RegexOptions.Singleline);
    }
}