namespace UrlShortener.Api.Options;

public class RedisOptions
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; set; }
    public int ShortUrlTtlSeconds { get; set; } = 3600;
    
    public TimeSpan ShortUrlTtl =>
        TimeSpan.FromSeconds(ShortUrlTtlSeconds);
}