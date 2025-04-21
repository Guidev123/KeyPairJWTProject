using Microsoft.Extensions.DependencyInjection;

namespace KeyPairJWT.Core.Interfaces;

public interface IJwksBuilder
{
    IServiceCollection Services { get; }
}
