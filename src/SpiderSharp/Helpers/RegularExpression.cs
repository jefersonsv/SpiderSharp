using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SpiderSharp.Helpers
{
    public static class RegeLibrary
    {
        public static readonly Regex Cookies = new Regex(@"(.*?)=(.*?)($|;|,(?! ))", RegexOptions.Singleline);
    }
}
