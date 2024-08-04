using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace WebWaves.Server.Infrastructure;
public class CacheProvider
{
    private static readonly Lazy<CacheProvider> lazyInstance = new Lazy<CacheProvider>(() => new CacheProvider(new MemoryCache(new MemoryCacheOptions())));

    public static CacheProvider Instance => lazyInstance.Value;

    private readonly IMemoryCache _memoryCache;

    private CacheProvider(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void RemoveFromCache(string key)
    {
        _memoryCache.Remove(key);
    }

    public void AddToCache<T>(string key, T value, TimeSpan expiration)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };
        _memoryCache.Set(key, value, cacheEntryOptions);
    }
    public T GetFromCache<T>(string key)
    {
        if (_memoryCache.TryGetValue(key, out T value))
        {
            return value;
        }
        return default(T);
    }
    
    public string GenerateUniqueId()
    {
        return Guid.NewGuid().ToString();
    }

}

