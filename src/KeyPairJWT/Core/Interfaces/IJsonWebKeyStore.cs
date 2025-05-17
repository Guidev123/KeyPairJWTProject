using KeyPairJWT.Core.Jwa;
using KeyPairJWT.Core.Models;
using System.Collections.ObjectModel;

namespace KeyPairJWT.Core.Interfaces;

public interface IJsonWebKeyStore
{
    Task Store(KeyMaterial keyMaterial);

    Task<KeyMaterial> GetCurrent(JwtType jwtKeyType = JwtType.Jws);

    Task Revoke(KeyMaterial keyMaterial, string reason = default);

    Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int quantity, JwtType? jwtKeyType = null);

    Task<KeyMaterial> Get(string keyId);

    Task Clear();
}