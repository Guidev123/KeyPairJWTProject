using KeyPairJWT.Core.DefaultStore;
using KeyPairJWT.Core.Interfaces;
using KeyPairJWT.Core.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace KeyPairJWT.Core
{
    public static class JsonWebKeySetManagerDependencyInjection
    {
        public static IJwksBuilder AddJwksManager(this IServiceCollection services, Action<JwtOptions> action = null)
        {
            if (action != null)
                services.Configure(action);

            services.AddDataProtection();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IJsonWebKeyStore, DataProtectionStore>();

            return new JwksBuilder(services);
        }

        public static IJwksBuilder PersistKeysInMemory(this IJwksBuilder builder)
        {
            builder.Services.AddScoped<IJsonWebKeyStore, InMemoryStore>();

            return builder;
        }
    }
}
