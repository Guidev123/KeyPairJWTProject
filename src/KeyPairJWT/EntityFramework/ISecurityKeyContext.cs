using KeyPairJWT.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyPairJWT.EntityFramework;

public interface ISecurityKeyContext
{
    DbSet<KeyMaterial> SecurityKeys { get; set; }
}
