using StackExchange.Redis;

namespace Web1.Service.Redis
{
    public interface IRedisService
    {
        public Task<bool> IsCheckListRedis(string typeList, string key);

        public Task RemoveToRedisAsync(string typeList, string key);

        public Task SetValueRedisAsync(string typeList ,string key, string value, TimeSpan? expiry = null);

        public Task<string> GetValueRedisAsync(string typeList, string key);
    }
}
