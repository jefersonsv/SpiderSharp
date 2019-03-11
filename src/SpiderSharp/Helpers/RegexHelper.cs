using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SpiderSharp.Helpers
{
    public static class RegexHelper
    {
        public static string RemoveAllNthChild(string url)
        {
            return Regex.Replace(url, @":nth-child\([0-9]+\)", string.Empty);
        }
    }
}
