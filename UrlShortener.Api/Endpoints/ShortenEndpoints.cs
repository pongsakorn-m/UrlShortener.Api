using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Endpoints;

public static class ShortenEndpoints
{
    public static IEndpointRouteBuilder MapShortenEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/shorten", async (
            string url,
            HttpContext httpContext,
            AppDbContext db) =>
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out _)) 
                return Results.BadRequest("Invalid URL");
    
            string shortCode;
            do
            {
                shortCode = GenerateShortCode();
            }
            while (await db.ShortUrls.AnyAsync(x => x.ShortCode == shortCode));

            var entity = new ShortUrl
            {
                ShortCode = shortCode,
                OriginalUrl = url
            };

            db.ShortUrls.Add(entity);
            await db.SaveChangesAsync();
    
            var baseUrl =
                $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

            return Results.Ok(new
            {
                shortUrl = $"{baseUrl}/{shortCode}"
            });
        });

        return app;
    }
    
    private static string GenerateShortCode(int length = 6)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }
}