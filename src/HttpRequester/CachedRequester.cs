using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

namespace HttpRequester
{
    public class CachedRequester
    {
        readonly Data.Redis.RedisConnection redis;
        readonly Requester requester;
        TimeSpan? duration;
        public string Cookies { get; private set; }

        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();

        public CachedRequester(string redisConnectionString, EnumHttpProvider httpProvider, TimeSpan? duration)
        {
            requester = new Requester(httpProvider);
            redis = new Data.Redis.RedisConnection(redisConnectionString);
            this.duration = duration;
        }

        public async Task<string> GetContentAsync(string url)
        {
            if (DefaultHeaders.Any())
                requester.DefaultHeaders = DefaultHeaders;

            var key = $"content:{url}";
            var source = await redis.DB.StringGetAsync(key);
            if (!source.HasValue)
            {
                try
                {
                    // first time
                    var response = await requester.GetContentAsync(url);
                    Cookies = requester.Cookies;
                    if (!string.IsNullOrEmpty(response))
                    {
                        TimeSpan dur = DateTime.UtcNow.AddMonths(1) - DateTime.UtcNow;
                        if (duration == null)
                            duration = dur;

                        var ret = await redis.DB.StringSetAsync(key, response, duration);
                        Log.Debug("Redis saved - Key: {key} Result: {ret}", key, ret);
                        return response;
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                {
                    return string.Empty;
                }
            }
            else
            {
                // return cached
                Log.Debug("Get cached: {key}", key);
                return source.ToString();
            }

            return null;
        }
    }
}