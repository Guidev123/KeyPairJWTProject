using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace KeyPairJWT.AspNet;

public class JwtPostConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public JwtPostConfigureOptions(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(new JwtServiceValidationHandler(_serviceProvider));
    }
}