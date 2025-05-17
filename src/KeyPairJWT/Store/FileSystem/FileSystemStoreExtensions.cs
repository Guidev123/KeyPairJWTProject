using KeyPairJWT.Core;
using KeyPairJWT.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KeyPairJWT.Store.FileSystem
{
    public static class FileSystemStoreExtensions
    {
        public static IJwksBuilder PersistKeysToFileSystem(this IJwksBuilder builder, DirectoryInfo directory)
        {
            builder.Services.AddScoped<IJsonWebKeyStore, FileSystemStore>(provider => new FileSystemStore(directory, provider.GetRequiredService<IOptions<JwtOptions>>(), provider.GetRequiredService<IMemoryCache>()));

            return builder;
        }
    }
}