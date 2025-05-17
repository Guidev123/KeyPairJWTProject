using KeyPairJWT.Core;
using KeyPairJWT.Core.Interfaces;
using KeyPairJWT.Core.Jwa;
using KeyPairJWT.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeyPairJWT.Store.FileSystem
{
    public class FileSystemStore : IJsonWebKeyStore
    {
        internal const string DefaultRevocationReason = "Revoked";
        private readonly IOptions<JwtOptions> _options;
        private readonly IMemoryCache _memoryCache;
        public DirectoryInfo KeysPath { get; }

        public FileSystemStore(DirectoryInfo keysPath, IOptions<JwtOptions> options, IMemoryCache memoryCache)
        {
            _options = options;
            _memoryCache = memoryCache;
            KeysPath = keysPath;
            if (!KeysPath.Exists)
                KeysPath.Create();
        }

        private string GetCurrentFile(JwtType jwtKeyType)
        {
            var files = Directory.GetFiles(KeysPath.FullName, $"*current*.{jwtKeyType}.key");
            if (files.Any())
                return files.First();

            return Path.Combine(KeysPath.FullName, $"{_options.Value.KeyPrefix}current.{jwtKeyType}.key");
        }

        public async Task Store(KeyMaterial securityParamteres)
        {
            if (!KeysPath.Exists)
                KeysPath.Create();

            JwtType keyType = securityParamteres.Use.Equals("enc", StringComparison.InvariantCultureIgnoreCase) ? JwtType.Jwe : JwtType.Jws;

            if (File.Exists(GetCurrentFile(keyType)))
                File.Copy(GetCurrentFile(keyType), Path.Combine(KeysPath.FullName, $"{_options.Value.KeyPrefix}old-{DateTime.Now:yyyy-MM-dd}-{securityParamteres.KeyId}.key"));

            await File.WriteAllTextAsync(Path.Combine(KeysPath.FullName, $"{_options.Value.KeyPrefix}current-{securityParamteres.KeyId}.{keyType}.key"),
                JsonSerializer.Serialize(securityParamteres, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
            ClearCache();
        }

        public bool NeedsUpdate(KeyMaterial current)
        {
            JwtType keyType = current.Use.Equals("enc", StringComparison.InvariantCultureIgnoreCase) ? JwtType.Jwe : JwtType.Jws;

            return !File.Exists(GetCurrentFile(keyType)) || File.GetCreationTimeUtc(GetCurrentFile(keyType)).AddDays(_options.Value.DaysUntilExpire) < DateTime.UtcNow.Date;
        }

        public async Task Revoke(KeyMaterial securityKeyWithPrivate, string reason = null)
        {
            if (securityKeyWithPrivate == null)
                return;

            securityKeyWithPrivate?.Revoke(reason ?? DefaultRevocationReason);
            foreach (var fileInfo in KeysPath.GetFiles("*.key"))
            {
                var key = GetKey(fileInfo.FullName);
                if (key.Id != securityKeyWithPrivate?.Id) continue;
                await File.WriteAllTextAsync(fileInfo.FullName, JsonSerializer.Serialize(securityKeyWithPrivate, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
                break;
            }
            ClearCache();
        }

        public Task<KeyMaterial?> GetCurrent(JwtType jwtKeyType = JwtType.Jws)
        {
            var cacheKey = JwkContants.CurrentJwkCache + jwtKeyType;

            if (!_memoryCache.TryGetValue(cacheKey, out KeyMaterial credentials))
            {
                credentials = GetKey(GetCurrentFile(jwtKeyType));

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_options.Value.CacheTime);
                if (credentials != null)
                    _memoryCache.Set(cacheKey, credentials, cacheEntryOptions);
            }

            return Task.FromResult(credentials);
        }

        private KeyMaterial? GetKey(string file)
        {
            if (!File.Exists(file)) return null;
            var keyParams = JsonSerializer.Deserialize<KeyMaterial>(File.ReadAllText(file));
            return keyParams!;
        }

        public Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int quantity = 5, JwtType? jwtKeyType = null)
        {
            var cacheKey = JwkContants.JwksCache + jwtKeyType;

            if (!_memoryCache.TryGetValue(cacheKey, out IReadOnlyCollection<KeyMaterial> keys))
            {
                var type = jwtKeyType == null ? "*" : jwtKeyType.ToString();

                keys = KeysPath.GetFiles($"*.{type}.key")
                    .Select(s => s.FullName)
                    .Select(GetKey).ToList().AsReadOnly();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_options.Value.CacheTime);

                if (keys.Any())
                    _memoryCache.Set(cacheKey, keys, cacheEntryOptions);
            }

            return Task.FromResult(keys
                         .GroupBy(s => s.Use)
                         .SelectMany(g => g.Take(quantity))
                         .ToList().AsReadOnly());
        }

        public Task<KeyMaterial?> Get(string keyId)
        {
            var files = Directory.GetFiles(KeysPath.FullName, $"*{keyId}*.key");
            if (files.Any())
                return Task.FromResult(GetKey(files.First()))!;

            return Task.FromResult(null as KeyMaterial);
        }

        public Task Clear()
        {
            if (KeysPath.Exists)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                foreach (var fileInfo in KeysPath.GetFiles($"*.key"))
                {
                    fileInfo.Delete();
                }
            }

            return Task.CompletedTask;
        }

        private void ClearCache()
        {
            _memoryCache.Remove(JwkContants.JwksCache);
            _memoryCache.Remove(JwkContants.JwksCache + JwtType.Jws);
            _memoryCache.Remove(JwkContants.JwksCache + JwtType.Jwe);
            _memoryCache.Remove(JwkContants.CurrentJwkCache);
            _memoryCache.Remove(JwkContants.CurrentJwkCache + JwtType.Jws);
            _memoryCache.Remove(JwkContants.CurrentJwkCache + JwtType.Jwe);
        }
    }
}