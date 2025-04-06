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

        public async Task<string> GetStringRedisAsync(string typeList, string key)
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

        public async Task SetAddRedisAsync(string typeList, string key, string value, TimeSpan? expiry = null)
        {
            if (expiry == null)
            {
                await _redisDb.SetAddAsync(typeList + key, value);
            }
            else
            {
                await _redisDb.KeyExpireAsync(typeList + key, expiry);
            }
        }

        public async Task<List<string>> GetSetMembersAsync(string typeList, string key)
        {
            var members = await _redisDb.SetMembersAsync(typeList + key);
            return members.Select(x => x.ToString()).ToList();
        }

        public async Task RemoveSetRedisAsync(string typeList, string key, string value)
        {
            await _redisDb.SetRemoveAsync(typeList + key, value);
        }

        public async Task SetStringRedisAsync(string typeList, string key, string value, TimeSpan? expiry = null)
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
