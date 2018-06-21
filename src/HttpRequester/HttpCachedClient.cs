using System;
using System.Threading.Tasks;

namespace HttpRequester
{
    public class HttpCachedClient
    {
        readonly Data.Redis.RedisConnection redis;

        public HttpCachedClient(string redisConnectionString)
        {
            redis = new Data.Redis.RedisConnection(redisConnectionString);
        }

        public async Task<string> GetContentAsync(string url)
        {
            var key = $"content:{url}";
            var source = await redis.DB.StringGetAsync(key);
            if (!source.HasValue)
            {
                // first time
                HttpClient client = new HttpClient();
                var response = await client.GetContentAsync(url);
                if (!string.IsNullOrEmpty(response))
                {
                    await redis.DB.StringSetAsync(key, response, DateTime.UtcNow.AddMonths(1) - DateTime.UtcNow);
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