using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using UrlShortener.Api.Data;
using UrlShortener.Api.Infrastructure.Redis;
using UrlShortener.Api.Options;

namespace UrlShortener.Api.Endpoints;

public static class RedirectEndpoints
{
    public static IEndpointRouteBuilder MapRedirectEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/{shortCode}", async (
            string shortCode,
            AppDbContext db,
            IRedisCacheService cache,
            IOptions<RedisOptions> redisOptions) =>
        {
            var cacheKey = $"shorturl:{shortCode}";

            var cacheUrl = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheUrl))
                return Results.Redirect(cacheUrl, false);

            var entry = await db.ShortUrls.FindAsync(shortCode);

            if (entry is null)
                return Results.NotFound();

            if (entry.ExpiresAt != null && entry.ExpiresAt < DateTime.UtcNow)
                return Results.NotFound();

            entry.ClickCount++;
            await db.SaveChangesAsync();

            await cache.SetStringAsync(
                cacheKey,
                entry.OriginalUrl,
                redisOptions.Value.ShortUrlTtl);

            return Results.Redirect(entry.OriginalUrl, false);
        });

        return app;
    }
}