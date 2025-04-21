using KeyPairJWT.Core.Models;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;

namespace KeyPairJWT.Core.Interfaces;

public interface IJwtService
{
    Task<SecurityKey> GenerateKey();
    Task<SecurityKey> GetCurrentSecurityKey();
    Task<SigningCredentials> GetCurrentSigningCredentials();
    Task<EncryptingCredentials> GetCurrentEncryptingCredentials();
    Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int? i = null);
    Task RevokeKey(string keyId, string reason = null);
    Task<SecurityKey> GenerateNewKey();
}
