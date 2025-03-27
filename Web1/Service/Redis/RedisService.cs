using StackExchange.Redis;

namespace Web1.Service.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redisDb;

        public RedisService(IConnectionMultiplexer redis) 
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task<string> GetValueRedisAsync(string typeList, string key)
        {
            var value = await _redisDb.StringGetAsync(typeList + key);
            return value.HasValue ? value.ToString() : string.Empty;
        }

        public async Task<bool> IsCheckListRedis(string typeList, string key)
        {
            var checkBan = await _redisDb.KeyExistsAsync(typeList + key);
            if(checkBan == true)
            {
                return true;
            }
            return false;
        }

        public async Task RemoveToRedisAsync(string typeList, string key)
        {
            await _redisDb.KeyDeleteAsync(typeList + key);
        }

        public async Task SetValueRedisAsync(string typeList, string key, string value, TimeSpan? expiry = null)
        {
            if (expiry == null) 
            {
                await _redisDb.StringSetAsync(typeList + key, value);
            }
            else
            {
                await _redisDb.StringSetAsync(typeList + key, value, expiry);
            }
        }
    }
}
