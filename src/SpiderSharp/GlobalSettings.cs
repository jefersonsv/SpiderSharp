using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp
{
    public static class GlobalSettings
    {
        public static string RedisConnectionString { get; set; }
        public static string MongoConnectionString { get; set; }
        public static string MongoDatabase { get; set; }

        public static bool UseRedisCache { get; set; }
    }
}
