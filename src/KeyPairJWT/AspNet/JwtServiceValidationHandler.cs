using KeyPairJWT.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KeyPairJWT.AspNet;

public class JwtServiceValidationHandler(IServiceProvider serviceProvider) : JwtSecurityTokenHandler
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();

        var keyMaterialTask = jwtService.GetLastKeys();
        Task.WaitAll(keyMaterialTask);
        validationParameters.IssuerSigningKeys = keyMaterialTask.Result.Select(s => s.GetSecurityKey());

        return base.ValidateToken(token, validationParameters, out validatedToken);
    }
}