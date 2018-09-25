using System;
using System.Collections.Generic;
using System.Text;
using HttpRequester;

namespace SpiderSharp
{
    public static class GlobalSettings
    {
        public static string RedisConnectionString { get; set; }
        public static string MongoConnectionString { get; set; }

        public static string ElasticSearchIndex { get; set; }
        public static string ElasticSearchConnectionString { get; set; }
        public static string MongoDatabase { get; set; }

        public static bool? UseRedisCache { get; set; }
        public static TimeSpan? RedisCacheDuration { get; set; }

        public static EnumHttpProvider? HttpProvider { get; set; }
        public static Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();
    }
}
