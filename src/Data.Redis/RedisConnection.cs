using StackExchange.Redis;

namespace Data.Redis
{
    public class RedisConnection
    {
        private ConnectionMultiplexer redis;

        public RedisConnection(string redisConnectionString)
        {
            redis = ConnectionMultiplexer.Connect(redisConnectionString);
            DB = redis.GetDatabase();
        }

        public IDatabase DB { get; private set; }
    }
}