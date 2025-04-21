using KeyPairJWT.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KeyPairJWT.Core;

public class JwksBuilder : IJwksBuilder
{
    public IServiceCollection Services { get; }

    public JwksBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }
}
