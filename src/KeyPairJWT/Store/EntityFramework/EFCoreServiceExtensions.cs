using KeyPairJWT.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KeyPairJWT.Store.EntityFramework;

public static class EFCoreServiceExtensions
{
    public static IJwksBuilder PersistKeysToDatabaseStore<TContext>(this IJwksBuilder builder)
        where TContext : DbContext, ISecurityKeyContext
    {
        builder.Services.AddScoped<IJsonWebKeyStore, DatabaseJsonWebKeyStore<TContext>>();

        return builder;
    }
}
