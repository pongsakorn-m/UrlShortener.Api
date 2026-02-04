using StackExchange.Redis;

namespace UrlShortener.Api.Infrastructure.Redis;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _db;
    
    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }


    public async Task<string?> GetStringAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? ttl = null)
    {
        await _db.StringSetAsync(key, value, ttl);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }

    public async Task<long> IncrementAsync(string key, TimeSpan? ttl = null)
    {
        var count = await _db.StringIncrementAsync(key);

        if (count == 1 && ttl.HasValue)
        {
            await _db.KeyExpireAsync(key, ttl);
        }

        return count;
    }

    public async Task<long> DecrementAsync(string key)
    {
        return await _db.StringDecrementAsync(key);
    }
}