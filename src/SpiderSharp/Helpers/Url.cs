using System;

namespace SpiderSharp.Helpers
{
    public static class Url
    {
        public static string ReplaceQueryString(string url, string newQueryString)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            UriBuilder uri = new UriBuilder(url);
            uri.Query = newQueryString;

            return uri.Uri.ToString();
        }

        public static string ReplaceBookmark(string url, string newQueryString)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            UriBuilder uri = new UriBuilder(url);
            uri.Fragment = newQueryString;

            return uri.Uri.ToString();
        }

        public static string RemoveQueryString(string url)
        {
            return ReplaceQueryString(url, string.Empty);
        }
    }
}