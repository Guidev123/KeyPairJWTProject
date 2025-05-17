using KeyPairJWT.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyPairJWT.Store.EntityFramework;

public interface ISecurityKeyContext
{
    DbSet<KeyMaterial> SecurityKeys { get; set; }
}
