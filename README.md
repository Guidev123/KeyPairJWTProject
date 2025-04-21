<body>
<p align="center">
  <a href="https://dotnet.microsoft.com/" target="_blank">
    <img src="https://upload.wikimedia.org/wikipedia/commons/e/ee/.NET_Core_Logo.svg" width="120" alt=".NET Logo" />
  </a>
</p>

<h1 align="center">KeyPairJWT Library</h1>

  <strong>KeyPairJWT</strong> is a lightweight and flexible .NET library designed to simplify the generation and validation of JSON Web Tokens (JWT) and JSON Web Key Sets (JWKS) using asymmetric cryptography. It supports secure token-based authentication for .NET applications, with built-in integration for ASP.NET Core and Entity Framework Core.

<p align="center">
        <a href="https://www.nuget.org/packages/KeyPairJWT">
            <img class="badge" src="https://img.shields.io/nuget/v/KeyPairJWT?color=purple&label=NuGet" alt="NuGet version" />
        </a>
        <a href="https://www.nuget.org/packages/KeyPairJWT">
            <img class="badge" src="https://img.shields.io/nuget/dt/KeyPairJWT?color=blue" alt="NuGet downloads" />
        </a>
        <img class="badge" src="https://img.shields.io/badge/.NET-6.0%20|%207.0%20|%208.0%20|%209.0-blueviolet" alt=".NET Support" />
        <img class="badge" src="https://img.shields.io/badge/license-MIT-green.svg" alt="License: MIT" />
        <img class="badge" src="https://img.shields.io/badge/status-stable-brightgreen" alt="Project Status: Stable" />
</p>


  <p>Key features:</p>
    <ul>
        <li>Generate and validate JWTs using asymmetric keys (e.g., ECDSA, RSA).</li>
        <li>Manage JWKS endpoints for public key distribution.</li>
        <li>Persist security keys to a database using Entity Framework Core.</li>
        <li>Support for .NET 6.0, 7.0, 8.0, and 9.0.</li>
        <li>Easy integration with ASP.NET Core authentication middleware.</li>
    </ul>

   <h2>Installation</h2>
   <p>Install the <code>KeyPairJWT</code> package via NuGet:</p>
    <pre><code>dotnet add package KeyPairJWT --version 1.0.0</code></pre>
    <p>Find the latest version on <a href="https://www.nuget.org/packages/KeyPairJWT">NuGet.org</a>.</p>

   <h2>Prerequisites</h2>
    <ul>
        <li><strong>.NET SDK</strong>: Version 6.0, 7.0, 8.0, or 9.0.</li>
        <li><strong>Dependencies</strong>:
            <ul>
                <li><code>Microsoft.AspNetCore.Authentication.JwtBearer</code></li>
                <li><code>Microsoft.AspNetCore.Http.Abstractions</code></li>
                <li><code>Microsoft.EntityFrameworkCore</code> (for key persistence)</li>
            </ul>
        </li>
        <li>A database provider compatible with Entity Framework Core (e.g., SQL Server, SQLite).</li>
    </ul>

  <h2>Configuration</h2>

  <h3>1. Client-Side Configuration (Token Validation)</h3>
    <p>To validate JWTs in your application, configure the ASP.NET Core authentication middleware to use the JWKS endpoint.</p>
    
   <h4>Program.cs</h4>
   
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add JWT validation services
builder.Services.AddJwtConfiguration(builder.Configuration);

var app = builder.Build();

// Use authentication middleware
app.UseAuthConfiguration();

app.Run();
```

  <h4>appsettings.json</h4>
  
```json
  "JwksSettings": {
    "JwksEndpoint": "https://your-issuer.com/jwks"
  }
```
      
<p><strong>Note</strong>: Replace <code>https://your-issuer.com/jwks</code> with the actual JWKS endpoint provided by your token issuer.</p>

   <h3>2. Token Issuer-Side Configuration (Token Generation)</h3>
    <p>To generate JWTs and expose a JWKS endpoint, configure the key management and persistence services.</p>

  <h4>Program.cs</h4>
  
```csharp
builder.Services.AddJwksManager(x => x.Jws = Algorithm.Create(DigitalSignaturesAlgorithm.EcdsaSha256))
    .PersistKeysToDatabaseStore<AuthenticationDbContext>()
    .UseJwtValidation();

app.UseAuthConfiguration();
app.UseJwksDiscovery();
```

<h4>AuthenticationDbContext.cs</h4>
    <p>Define a database context to store security keys and refresh tokens.</p>

```csharp
public class AuthenticationDbContext : DbContext, ISecurityKeyContext
{
    public AuthenticationDbContext(DbContextOptions&lt;AuthenticationDbContext&gt; options)
        : base(options)
    {
    }

  public DbSet<KeyMaterial> SecurityKeys { get; set; }
  public DbSet<RefreshToken> RefreshTokens { get; set; }

   protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```
 <p><strong>Note</strong>: Ensure your database provider (e.g., SQL Server, SQLite) is configured in <code>Program.cs</code> and migrations are applied.</p>

   <h2>Example Usage</h2>
    <p>Here is an example of using KeyPairJWT: https://github.com/Guidev123/EA-Identity</p>
    
  <h2>License</h2>
    <p>This project is licensed under the <a href="https://opensource.org/licenses/MIT">MIT License</a>.</p>

  <hr>
    <p class="center">Built with ❤️ for the .NET community.</p>
</body>
