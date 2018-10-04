using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp.Helpers
{
    public static class Url
    {
        public static string ReplaceQueryString(string url, string newQueryString)
        {
            UriBuilder uri = new UriBuilder(url);
            uri.Query = newQueryString;

            return uri.Uri.ToString();
        }
    }
}
