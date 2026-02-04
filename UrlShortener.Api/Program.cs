using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UrlShortener.Api.Data;
using UrlShortener.Api.Endpoints;
using UrlShortener.Api.Infrastructure.Redis;
using UrlShortener.Api.Middleware;
using UrlShortener.Api.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddOptions<RedisOptions>()
    .Bind(builder.Configuration.GetSection(RedisOptions.SectionName))
    .Validate(o => o.ShortUrlTtlSeconds > 0,
        "Redis TTL must be greater than zero")
    .ValidateOnStart();

builder.Services
    .AddOptions<RateLimitOptions>()
    .Bind(builder.Configuration.GetSection(RateLimitOptions.SectionName))
    .Validate(o => o.PermitLimit > 0)
    .Validate(o => o.WindowSeconds > 0)
    .ValidateOnStart();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var conn = builder.Configuration
        .GetSection(RedisOptions.SectionName)
        .GetValue<string>("ConnectionString");

    return ConnectionMultiplexer.Connect(conn);
});

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RateLimitMiddleware>();

app.MapGet("/", () => "URL Shortener API is running");

app.MapShortenEndpoints();
app.MapRedirectEndpoints();

app.Run();
