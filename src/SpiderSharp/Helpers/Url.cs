using System;
using System.Web;

namespace SpiderSharp.Helpers
{
    public static class Url
    {
        public static string SetBaseUrl(string baseUrl, string url)
        {
            baseUrl = baseUrl.Trim().TrimEnd('/');
            url = url.Trim().TrimStart('/');

            if (!url.Trim().ToLower().StartsWith(baseUrl.Trim().ToLower()))
            {
                if (Uri.TryCreate($"{baseUrl}/{url}", UriKind.RelativeOrAbsolute, out Uri uri))
                    return uri.ToString();

                throw new Exception("The baseurl cannot be set");
            }

            return url;
        }

        public static string ReplaceQueryString(string url, string newQueryString)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            UriBuilder uri = new UriBuilder(url);
            uri.Query = newQueryString;
            
            return System.Web.HttpUtility.UrlPathEncode(uri.Uri.ToString());
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

        public static string SetQueryString(string url, string queryStringKey, string queryStringValue)
        {
            var uri = new UriBuilder(url);

            var qs = HttpUtility.ParseQueryString(uri.Query);
            qs.Set(queryStringKey, queryStringValue);

            uri.Query = qs.ToString();

            return System.Web.HttpUtility.UrlPathEncode(uri.Uri.ToString());
        }
    }
}