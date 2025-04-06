using StackExchange.Redis;

namespace Web1.Service.Redis
{
    public interface IRedisService
    {
        public Task<bool> IsCheckListRedis(string typeList, string key);

        public Task RemoveToRedisAsync(string typeList, string key);

        public Task SetStringRedisAsync(string typeList, string key, string value, TimeSpan? expiry = null);

        public Task<string> GetStringRedisAsync(string typeList, string key);

        public Task SetAddRedisAsync(string typeList, string key, string value, TimeSpan? expiry = null);

        public Task<List<string>> GetSetMembersAsync(string typeList, string key);

        public Task RemoveSetRedisAsync(string typeList, string key, string value);

    }
}
