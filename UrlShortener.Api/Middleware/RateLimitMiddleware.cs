using Microsoft.Extensions.Options;
using UrlShortener.Api.Infrastructure.Redis;
using UrlShortener.Api.Options;

namespace UrlShortener.Api.Middleware;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;

    public RateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IRedisCacheService cache,
        IOptions<RateLimitOptions> options)
    {
        if (context.Request.Path.StartsWithSegments("/api/shorten"))
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"ratelimit:shorten:{ip}";

            var count = await cache.IncrementAsync(
                key,
                TimeSpan.FromSeconds(options.Value.WindowSeconds));

            if (count > options.Value.PermitLimit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return;
            }
        }

        await _next(context);
    }
}