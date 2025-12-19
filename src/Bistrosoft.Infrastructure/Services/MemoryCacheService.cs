using Microsoft.Extensions.Caching.Memory;

namespace Bistrosoft.Infrastructure.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(15);
    private readonly HashSet<string> _cacheKeys = new();

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (_cache.TryGetValue(key, out var cachedValue) && cachedValue is T typedValue)
        {
            return Task.FromResult<T?>(typedValue);
        }

        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
        };

        _cache.Set(key, value, options);
        _cacheKeys.Add(key);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        _cacheKeys.Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var keysToRemove = _cacheKeys
            .Where(key => key.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _cacheKeys.Remove(key);
        }

        return Task.CompletedTask;
    }
}

