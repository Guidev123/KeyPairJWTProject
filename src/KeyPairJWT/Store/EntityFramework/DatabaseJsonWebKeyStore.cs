﻿using KeyPairJWT.Core;
using KeyPairJWT.Core.Interfaces;
using KeyPairJWT.Core.Jwa;
using KeyPairJWT.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace KeyPairJWT.Store.EntityFramework;

internal class DatabaseJsonWebKeyStore<TContext> : IJsonWebKeyStore where TContext
             : DbContext, ISecurityKeyContext
{
    private readonly TContext _context;
    private readonly IOptions<JwtOptions> _options;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<DatabaseJsonWebKeyStore<TContext>> _logger;

    public DatabaseJsonWebKeyStore(TContext context,
                   ILogger<DatabaseJsonWebKeyStore<TContext>> logger,
                   IOptions<JwtOptions> options,
                   IMemoryCache memoryCache)
    {
        _context = context;
        _options = options;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task Store(KeyMaterial securityParamteres)
    {
        await _context.SecurityKeys.AddAsync(securityParamteres);

        _logger.LogInformation($"Saving new SecurityKeyWithPrivate {securityParamteres.Id}", typeof(TContext).Name);
        await _context.SaveChangesAsync();
        ClearCache();
    }

    public async Task<KeyMaterial> GetCurrent(JwtType jwtKeyType = JwtType.Jws)
    {
        if (!_memoryCache.TryGetValue(JwkContants.CurrentJwkCache, out KeyMaterial credentials))
        {
            credentials = await _context.SecurityKeys.Where(X => X.IsRevoked == false).OrderByDescending(d => d.CreationDate).AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync();
            credentials = await _context.SecurityKeys.Where(X => X.IsRevoked == false).OrderByDescending(d => d.CreationDate).AsNoTracking().FirstOrDefaultAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_options.Value.CacheTime);

            if (credentials != null)
                _memoryCache.Set(JwkContants.CurrentJwkCache, credentials, cacheEntryOptions);

            return credentials;
        }

        return credentials;
    }

    public async Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int quantity = 5, JwtType? jwtKeyType = null)
    {
        if (!_memoryCache.TryGetValue(JwkContants.JwksCache, out ReadOnlyCollection<KeyMaterial> keys))
        {
            keys = _context.SecurityKeys.OrderByDescending(d => d.CreationDate).Take(quantity).AsNoTrackingWithIdentityResolution().ToList().AsReadOnly();
            keys = _context.SecurityKeys.OrderByDescending(d => d.CreationDate).Take(quantity).AsNoTracking().ToList().AsReadOnly();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_options.Value.CacheTime);

            if (keys.Any())
                _memoryCache.Set(JwkContants.JwksCache, keys, cacheEntryOptions);

            return keys;
        }

        return keys;
    }

    public Task<KeyMaterial> Get(string keyId) => _context.SecurityKeys.FirstOrDefaultAsync(f => f.KeyId == keyId);

    public async Task Clear()
    {
        foreach (var securityKeyWithPrivate in _context.SecurityKeys)
        {
            _context.SecurityKeys.Remove(securityKeyWithPrivate);
        }

        await _context.SaveChangesAsync();
        ClearCache();
    }

    public async Task Revoke(KeyMaterial securityKeyWithPrivate, string reason = null)
    {
        if (securityKeyWithPrivate == null)
            return;

        securityKeyWithPrivate.Revoke(reason);
        _context.Attach(securityKeyWithPrivate);
        _context.SecurityKeys.Update(securityKeyWithPrivate);
        await _context.SaveChangesAsync();
        ClearCache();
    }

    private void ClearCache()
    {
        _memoryCache.Remove(JwkContants.JwksCache);
        _memoryCache.Remove(JwkContants.CurrentJwkCache);
    }
}