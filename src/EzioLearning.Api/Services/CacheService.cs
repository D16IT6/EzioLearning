using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace EzioLearning.Api.Services;

public class CacheService
{
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

    private string GenerateKey(string key, string? prefix)
    {
        return !string.IsNullOrEmpty(prefix) ? $"{prefix}-{key}" : key;
    }

    public TItem? Get<TItem>(string key, string? prefix = null) where TItem : class?
    {
        return _memoryCache.Get<TItem>(GenerateKey(key, prefix));
    }

    public TItem Set<TItem>(string key, TItem value, DateTimeOffset dateTimeOffset, string? prefix = null)
    {
        return _memoryCache.Set(GenerateKey(key, prefix), value, dateTimeOffset);
    }

    public TItem Set<TItem>(string key, TItem value, TimeSpan timeSpan, string? prefix = null)
    {
        return _memoryCache.Set(GenerateKey(key, prefix), value, timeSpan);
    }

    public TItem Set<TItem>(string key, TItem value, IChangeToken changeToken, string? prefix = null)
    {
        return _memoryCache.Set(GenerateKey(key, prefix), value, changeToken);
    }

    public void Remove(string key, string? prefix = null)
    {
        _memoryCache.Remove(GenerateKey(key, prefix));
    }

}