using KeyPairJWT.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KeyPairJWT.Core;

public class JwksBuilder(IServiceCollection services) : IJwksBuilder
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
}
