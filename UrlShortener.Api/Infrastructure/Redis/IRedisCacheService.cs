namespace UrlShortener.Api.Infrastructure.Redis;

public interface IRedisCacheService
{
    Task<string?> GetStringAsync(string key);

    Task SetStringAsync(
        string key,
        string value,
        TimeSpan? ttl = null);

    Task<bool> RemoveAsync(string key);

    Task<long> IncrementAsync(
        string key,
        TimeSpan? ttl = null);

    Task<long> DecrementAsync(string key);
}