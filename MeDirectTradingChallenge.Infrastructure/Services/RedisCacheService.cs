using MeDirectTradingChallenge.Domain.Services;
using StackExchange.Redis;

namespace MeDirectTradingChallenge.Infrastructure.Services
{
    public class RedisCacheService : ICacheService, IDisposable
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _cache;

        public RedisCacheService(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _cache = _redis.GetDatabase();
        }

        public async Task SetCacheAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var jsonData = System.Text.Json.JsonSerializer.Serialize(value);
            await _cache.StringSetAsync(key, jsonData, expiry);
        }

        public async Task<T> GetCacheAsync<T>(string key)
        {
            var jsonData = await _cache.StringGetAsync(key);
            if (jsonData.IsNullOrEmpty)
                return default;

            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonData);
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}
