﻿using System;
using System.Threading.Tasks;

namespace HttpRequester
{
    public class CachedRequester
    {
        readonly Data.Redis.RedisConnection redis;
        readonly Requester requester;
        TimeSpan? duration;
        public string Cookies { get; private set; }

        public CachedRequester(string redisConnectionString, EnumHttpProvider httpProvider, TimeSpan? duration)
        {
            requester = new Requester(httpProvider);
            redis = new Data.Redis.RedisConnection(redisConnectionString);
            this.duration = duration;
        }

        public async Task<string> GetContentAsync(string url)
        {
            var key = $"content:{url}";
            var source = await redis.DB.StringGetAsync(key);
            if (!source.HasValue)
            {
                // first time
                var response = await requester.GetContentAsync(url);
                Cookies = requester.Cookies;
                if (!string.IsNullOrEmpty(response))
                {
                    TimeSpan dur = DateTime.UtcNow.AddMonths(1) - DateTime.UtcNow;
                    if (duration == null)
                        duration = dur;

                    await redis.DB.StringSetAsync(key, response, duration);
                    return response;
                }
            }
            else
            {
                // return cached
                return source.ToString();
            }

            return null;
        }
    }
}