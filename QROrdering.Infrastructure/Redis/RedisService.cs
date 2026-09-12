using QROrdering.Application.Common.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace QROrdering.Infrastructure.Redis
{
    // CRUD và quản lý dữ liệu với Redis
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redis;
        private readonly IConnectionMultiplexer _connection;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisService(
            IConnectionMultiplexer connection)
        {
            _connection = connection;
            _redis = connection.GetDatabase();

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Lưu dữ liệu vào Redis.
        /// </summary>
        public async Task<bool> SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(
                value,
                _jsonOptions);

            return await _redis.StringSetAsync(
                key,
                json,
                expiry);
        }

        /// <summary>
        /// Lấy dữ liệu từ Redis.
        /// </summary>
        public async Task<T?> GetAsync<T>(
            string key)
        {
            var value = await _redis.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(
                value.ToString(),
                _jsonOptions);
        }

        /// <summary>
        /// Kiểm tra key có tồn tại không.
        /// </summary>
        public async Task<bool> ExistsAsync(
            string key)
        {
            return await _redis.KeyExistsAsync(key);
        }

        /// <summary>
        /// Xóa một hoặc nhiều key.
        /// </summary>
        public async Task<long> RemoveAsync(
            params string[] keys)
        {
            if (keys.Length == 0)
                return 0;

            var redisKeys = keys
                .Select(key => (RedisKey)key)
                .ToArray();

            return await _redis.KeyDeleteAsync(
                redisKeys);
        }

        /// <summary>
        /// Lấy thời gian sống còn lại của key.
        /// </summary>
        public async Task<TimeSpan?> GetTimeToLiveAsync(
            string key)
        {
            return await _redis.KeyTimeToLiveAsync(key);
        }

        /// <summary>
        /// Gia hạn thời gian sống của key.
        /// </summary>
        public async Task<bool> ExpireAsync(
            string key,
            TimeSpan expiry)
        {
            return await _redis.KeyExpireAsync(
                key,
                expiry);
        }

        /// <summary>
        /// Xóa tất cả key theo pattern.
        /// </summary>
        public async Task RemoveByPatternAsync(
            string pattern)
        {
            var keys = GetKeys(pattern).ToArray();

            if (keys.Length == 0)
                return;

            await RemoveAsync(keys);
        }

        /// <summary>
        /// Lấy danh sách key theo pattern.
        /// </summary>
        public IEnumerable<string> GetKeys(
            string pattern)
        {
            var endpoint = _connection
                .GetEndPoints()
                .First();

            var server = _connection
                .GetServer(endpoint);

            return server
                .Keys(pattern: pattern)
                .Select(key => key.ToString());
        }
    }
}