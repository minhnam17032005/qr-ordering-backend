using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace QROrdering.Infrastructure.Redis
{
    public class RedisService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        // SET String
        public async Task SetAsync(
            string key,
            string value,
            TimeSpan expiration)
        {
            var db = _redis.GetDatabase();

            await db.StringSetAsync(
                key,
                value,
                expiration);
        }

        // SET Object -> JSON
        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan expiration)
        {
            var db = _redis.GetDatabase();

            var json = JsonSerializer.Serialize(value);

            await db.StringSetAsync(
                key,
                json,
                expiration);
        }

        // GET JSON -> Object
        public async Task<T?> GetAsync<T>(string key)
        {
            var db = _redis.GetDatabase();

            var value = await db.StringGetAsync(key);

            if (!value.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(value.ToString());
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var db = _redis.GetDatabase();

            return await db.KeyExistsAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            var db = _redis.GetDatabase();

            await db.KeyDeleteAsync(key);
        }
    }
}