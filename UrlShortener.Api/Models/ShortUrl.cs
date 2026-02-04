namespace UrlShortener.Api.Models;

public class ShortUrl
{
    public string ShortCode { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public int ClickCount { get; set; }
}