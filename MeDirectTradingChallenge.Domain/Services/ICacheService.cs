namespace MeDirectTradingChallenge.Domain.Services
{
    public interface ICacheService
    {
        Task SetCacheAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T> GetCacheAsync<T>(string key);
        Task RemoveCacheAsync(string key);
    }
}
