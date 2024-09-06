using StackExchange.Redis;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Data.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redisConnection;

        public CacheService(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
        }

        public async Task<string> GetCacheAsync(string key)
        {
            var db = _redisConnection.GetDatabase();
            var value = await db.StringGetAsync(key);
            return value.ToString();
        }

        public async Task SetCacheAsync(string key, string value)
        {
            var db = _redisConnection.GetDatabase();
            await db.StringSetAsync(key, value);
        }

        public async Task RemoveCacheAsync(string key)
        {
            var db = _redisConnection.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
    }
}