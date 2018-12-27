using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Serilog;

namespace HttpRequester
{
    public class CachedRequester
    {
        readonly Data.Redis.RedisConnection redis;
        public readonly Requester requester;
        TimeSpan? duration;
        public string Cookies { get; private set; }

        public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>();

        public CachedRequester(Requester requester, string redisConnectionString, TimeSpan? duration)
        {
            this.requester = requester;
            this.duration = duration;
            redis = new Data.Redis.RedisConnection(redisConnectionString);
        }

        public static async Task DeleteContentAsync(string redisConnectionString, string url, string body)
        {
            var prefix = new Uri(url).Host;

            var c = new Data.Redis.RedisConnection(redisConnectionString);
            var key = GetKey(url, body); // $"content:{prefix}:{url}";
            await c.DB.KeyDeleteAsync(key);
        }

        public static string MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                return Encoding.ASCII.GetString(result);
            }
        }

        static string GetKey(string url, string body)
        {
            var uri = new Uri(url);
            var template = $"{uri.Scheme}:{uri.Host}:{url}";

            if (!string.IsNullOrEmpty(body))
            {
                template = template + "#" + MD5Hash(body);
            }

            return template;
            //return $"{AllReplace(url.ToString().Replace('/', ':'), ":")}";
        }

        static string AllReplace(string text, string character)
        {
            Regex regex = new Regex(character + "{2,}");
            return regex.Replace(text, character);
        }

        public async Task<RequestContent> GetContentAsync(string url, string body)
        {
            var prefix = new Uri(url).Host;

            if (DefaultHeaders.Any())
                requester.DefaultHeaders = DefaultHeaders;

            var key = GetKey(url, body); //$"content:{prefix}:{url}";

            var source = await redis.DB.StringGetAsync(key);
            if (!source.HasValue)
            {
                RetryPolicy retry = Policy
                    .Handle<System.AggregateException>(r => r.Message.Contains("400"))
                    .Or<HttpRequestException>(r => r.Message.Contains("400"))
                    .WaitAndRetryAsync(new[]
                    {
                      TimeSpan.FromSeconds(2),
                      TimeSpan.FromSeconds(8)
                    });

                // first time
                var response = await retry.ExecuteAsync(() =>
                {
                    return requester.GetContentAsync(url);
                });

                Cookies = requester.Cookies;
                if (!string.IsNullOrEmpty(response))
                {
                    Log.Information($"Loaded online: {url}");
                    TimeSpan dur = DateTime.UtcNow.AddMonths(1) - DateTime.UtcNow;
                    if (duration == null)
                        duration = dur;

                    var ret = await redis.DB.StringSetAsync(key, response, duration);
                    Log.Debug("Redis saved - Key: {key} Result: {ret}", key, ret);

                    return new RequestContent { Content = response, UsedCache = false };
                }
                
            }
            else
            {
                // return cached
                Log.Debug("Get cached: {key}", key);

                return new RequestContent { Content = source.ToString(), UsedCache = true };
            }

            return null;
        }
    }
}