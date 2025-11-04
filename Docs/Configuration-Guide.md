# Configuration Guide

Comprehensive guide to configuring CMouss.IdentityFramework for different scenarios.

## Table of Contents

- [Overview](#overview)
- [Basic Configuration](#basic-configuration)
- [Database Configuration](#database-configuration)
- [Authentication Configuration](#authentication-configuration)
- [Token Configuration](#token-configuration)
- [User Settings](#user-settings)
- [LDAP/Active Directory Configuration](#ldapactive-directory-configuration)
- [Advanced Configuration](#advanced-configuration)
- [Environment-Specific Configuration](#environment-specific-configuration)
- [Security Best Practices](#security-best-practices)

## Overview

The framework is configured through the `IDFManagerConfig` class, which must be set before using any framework services.

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    // Configuration options
});
```

## Basic Configuration

### Minimal Setup

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",
    TokenEncryptionKey = "YourSecretKey123"
});
```

### Recommended Production Setup

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    // Database
    DatabaseType = DatabaseType.MSSQL,
    DBConnectionString = "Server=localhost;Database=IdentityDB;Trusted_Connection=True;",
    DBLifeCycle = DBLifeCycle.InMemoryOnly,

    // Administrator
    AdministratorUserName = "admin",
    AdministratorPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD"),
    AdministratorRoleName = "Administrators",

    // Security
    TokenEncryptionKey = Environment.GetEnvironmentVariable("TOKEN_KEY"),
    DefaultTokenLifeTime = 120, // 2 hours
    TokenValidationMode = TokenValidationMode.DecryptAndValidate,
    AllowUserMultipleSessions = false,

    // User Defaults
    IsActiveByDefault = true,
    IsLockedByDefault = false,

    // Other
    IDGeneratorLevel = IDGeneratorLevel._128,
    DefaultListPageSize = 20
});
```

## Database Configuration

### SQL Server Configuration

#### Windows Authentication

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.MSSQL,
    DBConnectionString = "Server=localhost;Database=IdentityDB;Trusted_Connection=True;"
});
```

#### SQL Server Authentication

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.MSSQL,
    DBConnectionString = "Server=localhost;Database=IdentityDB;User Id=sa;Password=YourPassword;"
});
```

#### Azure SQL Database

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.MSSQL,
    DBConnectionString = "Server=tcp:yourserver.database.windows.net,1433;" +
                        "Initial Catalog=IdentityDB;" +
                        "Persist Security Info=False;" +
                        "User ID=yourusername;" +
                        "Password=yourpassword;" +
                        "MultipleActiveResultSets=False;" +
                        "Encrypt=True;" +
                        "TrustServerCertificate=False;" +
                        "Connection Timeout=30;"
});
```

### SQLite Configuration

#### File-based Database

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;"
});
```

#### With Full Path

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=C:\\Data\\identity.db;"
});
```

#### In-Memory SQLite (Testing)

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=:memory:;"
});
```

### Database Lifecycle

Control how database contexts are managed:

```csharp
public enum DBLifeCycle
{
    InMemoryOnly,      // Single shared instance (best performance)
    OnRequestOnly,     // New instance per request (isolated)
    Both              // Hybrid approach
}
```

#### InMemoryOnly (Recommended)

Best performance, single shared context.

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DBLifeCycle = DBLifeCycle.InMemoryOnly
});
```

#### OnRequestOnly

New context per operation, better for concurrent scenarios.

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DBLifeCycle = DBLifeCycle.OnRequestOnly
});
```

## Authentication Configuration

### Database Authentication (Default)

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    AuthenticationBackend = AuthenticationBackend.Database
});
```

### LDAP Authentication

See [LDAP/Active Directory Configuration](#ldapactive-directory-configuration) section.

## Token Configuration

### Token Encryption

**CRITICAL**: The `TokenEncryptionKey` must be:
- At least 8 characters long
- Kept secret
- Same across all application instances
- Never changed in production (invalidates all tokens)

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    TokenEncryptionKey = "YourVerySecretKey123456"
});
```

**Production Best Practice**:

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    TokenEncryptionKey = Environment.GetEnvironmentVariable("TOKEN_ENCRYPTION_KEY")
                        ?? throw new InvalidOperationException("TOKEN_ENCRYPTION_KEY not set")
});
```

### Token Lifetime

Set how long tokens remain valid (in minutes):

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DefaultTokenLifeTime = 60,        // User tokens: 1 hour
    DefaultAppAccessLifeTime = 120    // App tokens: 2 hours
});
```

**Recommendations**:
- **Short-lived apps** (mobile, SPA): 30-60 minutes
- **Standard web apps**: 60-120 minutes
- **Long-lived sessions**: 480 minutes (8 hours)
- **API services**: 1440 minutes (24 hours)

### Token Validation Modes

#### DecryptOnly (Fast)

Validates token by decryption only, no database lookup.

**Pros**: Fast, reduced database load
**Cons**: Can't detect manually revoked tokens

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    TokenValidationMode = TokenValidationMode.DecryptOnly
});
```

Use when:
- High performance is critical
- Token lifetime is short
- Token revocation is not required

#### DecryptAndValidate (Secure)

Validates token by decryption AND database lookup.

**Pros**: Detects revoked/expired tokens, more secure
**Cons**: Slower, more database load

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    TokenValidationMode = TokenValidationMode.DecryptAndValidate
});
```

Use when:
- Security is critical
- Need to revoke tokens manually
- Token lifetime is long

### Multiple Sessions

Control whether users can have multiple active sessions:

```csharp
// Allow multiple concurrent sessions
IDFManager.Configure(new IDFManagerConfig
{
    AllowUserMultipleSessions = true
});

// Single session only (new login invalidates previous)
IDFManager.Configure(new IDFManagerConfig
{
    AllowUserMultipleSessions = false
});
```

## User Settings

### Administrator Account

Configure the default admin account created by `InsertMasterData()`:

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    AdministratorUserName = "admin",
    AdministratorPassword = "SecurePassword123!",
    AdministratorRoleName = "Administrators",
    AdministratorRoleId = "admin-role-id"  // Optional, defaults to AdministratorRoleName
});
```

**Production Best Practice**:

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    AdministratorUserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? "admin",
    AdministratorPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
                           ?? throw new InvalidOperationException("ADMIN_PASSWORD must be set")
});
```

### User Creation Defaults

Set default values for new users:

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    IsActiveByDefault = true,   // New users are active by default
    IsLockedByDefault = false   // New users are not locked
});
```

**Email Verification Workflow**:

```csharp
// Users start inactive until email is verified
IDFManager.Configure(new IDFManagerConfig
{
    IsActiveByDefault = false,
    IsLockedByDefault = false
});
```

**Admin Approval Workflow**:

```csharp
// Users start locked until admin approval
IDFManager.Configure(new IDFManagerConfig
{
    IsActiveByDefault = true,
    IsLockedByDefault = true
});
```

### ID Generation

Configure the length of generated IDs (GUIDs):

```csharp
public enum IDGeneratorLevel
{
    _32,   // 32 characters
    _64,   // 64 characters
    _96,   // 96 characters
    _128   // 128 characters (most secure)
}
```

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    IDGeneratorLevel = IDGeneratorLevel._128  // Maximum security
});
```

**Recommendations**:
- **High security**: _128 (default)
- **Standard apps**: _64
- **Performance critical**: _32

### Pagination

Default page size for list operations:

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DefaultListPageSize = 20  // 20 items per page
});
```

## LDAP/Active Directory Configuration

### Basic LDAP Configuration

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    AuthenticationBackend = AuthenticationBackend.LDAP,

    // LDAP Server
    LDAPServerAddress = "ldap://ldap.company.com",
    LDAPServerPort = 389,
    LDAPUseSSL = false,

    // Bind Credentials
    LDAPBindDN = "CN=Admin,DC=company,DC=com",
    LDAPBindPassword = "admin_password",

    // Search Configuration
    LDAPSearchBase = "DC=company,DC=com",
    LDAPUserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))"
});
```

### Active Directory Configuration

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    AuthenticationBackend = AuthenticationBackend.LDAP,

    // Active Directory
    ADDomain = "company.com",
    ADServerAddress = "dc.company.com",

    // LDAP Settings
    LDAPBindDN = "CN=Service Account,CN=Users,DC=company,DC=com",
    LDAPBindPassword = Environment.GetEnvironmentVariable("AD_BIND_PASSWORD"),
    LDAPSearchBase = "DC=company,DC=com",
    LDAPUserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))",
    LDAPUseSSL = false
});
```

### Secure LDAP (LDAPS)

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    AuthenticationBackend = AuthenticationBackend.LDAP,
    LDAPServerAddress = "ldaps://ldap.company.com",
    LDAPServerPort = 636,
    LDAPUseSSL = true,
    LDAPBindDN = "CN=Admin,DC=company,DC=com",
    LDAPBindPassword = "password",
    LDAPSearchBase = "DC=company,DC=com",
    LDAPUserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))"
});
```

### LDAP Search Filters

Common search filter patterns:

#### Active Directory (sAMAccountName)

```csharp
LDAPUserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))"
```

#### OpenLDAP (uid)

```csharp
LDAPUserSearchFilter = "(&(objectClass=inetOrgPerson)(uid={0}))"
```

#### Email-based Login

```csharp
LDAPUserSearchFilter = "(&(objectClass=user)(mail={0}))"
```

#### Multiple Attributes

```csharp
LDAPUserSearchFilter = "(|(sAMAccountName={0})(mail={0})(userPrincipalName={0}))"
```

### Group Membership Filter

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    LDAPGroupSearchFilter = "(&(objectClass=group)(member={0}))"
});
```

## Advanced Configuration

### Complete Configuration Example

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    // Database Configuration
    DatabaseType = DatabaseType.MSSQL,
    DBConnectionString = builder.Configuration.GetConnectionString("IdentityDB"),
    DBLifeCycle = DBLifeCycle.InMemoryOnly,

    // ID Generation
    IDGeneratorLevel = IDGeneratorLevel._128,

    // Administrator Setup
    AdministratorRoleName = "Administrators",
    AdministratorRoleId = "admin-role",
    AdministratorUserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME"),
    AdministratorPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD"),

    // Pagination
    DefaultListPageSize = 20,

    // User Defaults
    IsActiveByDefault = true,
    IsLockedByDefault = false,

    // Token Configuration
    DefaultTokenLifeTime = 120,
    DefaultAppAccessLifeTime = 240,
    AllowUserMultipleSessions = false,
    TokenEncryptionKey = Environment.GetEnvironmentVariable("TOKEN_KEY"),
    TokenValidationMode = TokenValidationMode.DecryptAndValidate,

    // Authentication Backend
    AuthenticationBackend = AuthenticationBackend.Database
});
```

### Accessing Configuration at Runtime

```csharp
// Access current configuration
var config = IDFManager.config;

// Read specific settings
int tokenLifetime = config.DefaultTokenLifeTime;
string adminRole = config.AdministratorRoleName;
bool allowMultipleSessions = config.AllowUserMultipleSessions;
```

## Environment-Specific Configuration

### Development Environment

```csharp
if (builder.Environment.IsDevelopment())
{
    IDFManager.Configure(new IDFManagerConfig
    {
        DatabaseType = DatabaseType.SQLite,
        DBConnectionString = "Data Source=dev-identity.db;",
        AdministratorUserName = "admin",
        AdministratorPassword = "Admin@123",
        TokenEncryptionKey = "dev-secret-key",
        DefaultTokenLifeTime = 480,  // 8 hours for development
        TokenValidationMode = TokenValidationMode.DecryptOnly,
        AllowUserMultipleSessions = true
    });
}
```

### Production Environment

```csharp
if (builder.Environment.IsProduction())
{
    IDFManager.Configure(new IDFManagerConfig
    {
        DatabaseType = DatabaseType.MSSQL,
        DBConnectionString = builder.Configuration.GetConnectionString("IdentityDB"),
        AdministratorUserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME"),
        AdministratorPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD"),
        TokenEncryptionKey = Environment.GetEnvironmentVariable("TOKEN_ENCRYPTION_KEY"),
        DefaultTokenLifeTime = 60,
        TokenValidationMode = TokenValidationMode.DecryptAndValidate,
        AllowUserMultipleSessions = false,
        DBLifeCycle = DBLifeCycle.InMemoryOnly
    });
}
```

### Using appsettings.json

**appsettings.json**:

```json
{
  "IdentityFramework": {
    "DatabaseType": "MSSQL",
    "DefaultTokenLifeTime": 60,
    "AllowUserMultipleSessions": false,
    "IsActiveByDefault": true,
    "IsLockedByDefault": false,
    "DefaultListPageSize": 20
  },
  "ConnectionStrings": {
    "IdentityDB": "Server=localhost;Database=IdentityDB;Trusted_Connection=True;"
  }
}
```

**Program.cs**:

```csharp
var identityConfig = builder.Configuration.GetSection("IdentityFramework");

IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = Enum.Parse<DatabaseType>(identityConfig["DatabaseType"]),
    DBConnectionString = builder.Configuration.GetConnectionString("IdentityDB"),
    TokenEncryptionKey = Environment.GetEnvironmentVariable("TOKEN_KEY"),
    DefaultTokenLifeTime = int.Parse(identityConfig["DefaultTokenLifeTime"]),
    AllowUserMultipleSessions = bool.Parse(identityConfig["AllowUserMultipleSessions"]),
    IsActiveByDefault = bool.Parse(identityConfig["IsActiveByDefault"]),
    IsLockedByDefault = bool.Parse(identityConfig["IsLockedByDefault"]),
    DefaultListPageSize = int.Parse(identityConfig["DefaultListPageSize"]),
    AdministratorUserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME"),
    AdministratorPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
});
```

### Using Azure Key Vault

```csharp
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.MSSQL,
    DBConnectionString = (await client.GetSecretAsync("IdentityDB-ConnectionString")).Value.Value,
    TokenEncryptionKey = (await client.GetSecretAsync("TokenEncryptionKey")).Value.Value,
    AdministratorUserName = "admin",
    AdministratorPassword = (await client.GetSecretAsync("AdminPassword")).Value.Value
});
```

## Security Best Practices

### 1. Never Hardcode Secrets

```csharp
// BAD
TokenEncryptionKey = "my-secret-key"

// GOOD
TokenEncryptionKey = Environment.GetEnvironmentVariable("TOKEN_KEY")
```

### 2. Use Strong Token Encryption Keys

```csharp
// Generate a strong key (example)
// In PowerShell: [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))

TokenEncryptionKey = "Base64EncodedRandomKey=="
```

### 3. Set Appropriate Token Lifetimes

```csharp
// Short-lived for sensitive operations
DefaultTokenLifeTime = 30  // 30 minutes

// Standard for regular apps
DefaultTokenLifeTime = 60  // 1 hour

// Long-lived for background services
DefaultTokenLifeTime = 1440  // 24 hours
```

### 4. Use Secure Database Connections

```csharp
// SQL Server with encryption
DBConnectionString = "Server=localhost;Database=IdentityDB;" +
                    "User Id=app_user;Password=SecurePassword;" +
                    "Encrypt=True;TrustServerCertificate=False;"
```

### 5. Restrict Multiple Sessions for Sensitive Apps

```csharp
AllowUserMultipleSessions = false  // One session per user
```

### 6. Use DecryptAndValidate in Production

```csharp
TokenValidationMode = TokenValidationMode.DecryptAndValidate
```

### 7. Secure LDAP Connections

```csharp
LDAPServerAddress = "ldaps://ldap.company.com"
LDAPServerPort = 636
LDAPUseSSL = true
```

### 8. Use Service Accounts for LDAP

```csharp
// Don't use admin accounts
LDAPBindDN = "CN=Service Account,CN=Users,DC=company,DC=com"
LDAPBindPassword = Environment.GetEnvironmentVariable("LDAP_SERVICE_PASSWORD")
```

## Validation and Testing

### Validate Configuration

```csharp
private static void ValidateConfiguration()
{
    var config = IDFManager.config;

    if (string.IsNullOrEmpty(config.TokenEncryptionKey))
        throw new InvalidOperationException("TokenEncryptionKey is required");

    if (config.TokenEncryptionKey.Length < 8)
        throw new InvalidOperationException("TokenEncryptionKey must be at least 8 characters");

    if (string.IsNullOrEmpty(config.DBConnectionString))
        throw new InvalidOperationException("DBConnectionString is required");

    if (config.DefaultTokenLifeTime <= 0)
        throw new InvalidOperationException("DefaultTokenLifeTime must be positive");
}

// Call after configuration
IDFManager.Configure(config);
ValidateConfiguration();
```

### Test Database Connection

```csharp
try
{
    using var db = new IDFDBContext();
    db.Database.CanConnect();
    Console.WriteLine("Database connection successful");
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection failed: {ex.Message}");
}
```

---

[← Back to README](README.md) | [Authentication & Authorization →](Authentication-Authorization.md)
