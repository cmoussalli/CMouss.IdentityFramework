# CMouss.IdentityFramework

A full-featured identity and access management framework for .NET applications. Provides complete authentication, authorization, and user management with support for multiple authentication backends and Blazor UI components.

[![.NET](https://img.shields.io/badge/.NET-8.0%2B-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/CMouss.IdentityFramework)](https://www.nuget.org/packages/CMouss.IdentityFramework)

## Features

- **Multiple Authentication Backends**: Database or LDAP/Active Directory authentication
- **Role-Based Access Control (RBAC)**: Granular permissions with entity-action model
- **Blazor UI Components**: Ready-to-use login, signup, and logout components
- **API Protection**: Action filters for securing API endpoints
- **Token Management**: Encrypted tokens with configurable expiration
- **Multi-Tenant Support**: Application-level access control
- **Security**: AES encryption, password hashing, CSRF protection, session management

## Installation

Install via NuGet:

```bash
# Core Framework
dotnet add package CMouss.IdentityFramework

# Blazor UI Components (optional)
dotnet add package CMouss.IdentityFramework.BlazorUI

# API Serving (optional)
dotnet add package CMouss.IdentityFramework.API.Serving
```

## Quick Start

### 1. Configure the Framework

```csharp
using CMouss.IdentityFramework;

IDFManager.Configure(new IDFManagerConfig
{
    // Database
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",

    // Administrator Account
    AdministratorUserName = "admin",
    AdministratorPassword = "YourSecurePassword",
    AdministratorRoleName = "Administrators",

    // Security
    TokenEncryptionKey = "YourSecureEncryptionKey123",
    DefaultTokenLifeTime = new LifeTime(365, 0, 0),
    TokenValidationMode = TokenValidationMode.DecryptOnly,
    AllowUserMultipleSessions = false,

    // Defaults
    IsActiveByDefault = true,
    IsLockedByDefault = false,
    DBLifeCycle = DBLifeCycle.Both,
    DefaultListPageSize = 20
});

// Initialize Database
IDFDBContext db = new IDFDBContext();
db.Database.EnsureCreated();
db.InsertMasterData();
```

### 2. Blazor Server Setup

```csharp
using CMouss.IdentityFramework.BlazorUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register Identity Framework services
builder.Services.AddIdentityFrameworkBlazorUI();

var app = builder.Build();
```

### 3. Use Protected Pages

```razor
@page "/dashboard"
@layout AuthLayout

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }

    protected override void OnInitialized()
    {
        // Access authenticated user via authLayoutModel
        var userName = authLayoutModel.User.UserName;
    }
}
```

## Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `DatabaseType` | SQLite | Database provider (SQLite, MSSQL) |
| `DBConnectionString` | - | Connection string |
| `TokenEncryptionKey` | - | AES encryption key for tokens |
| `DefaultTokenLifeTime` | 1 min | Token validity duration |
| `TokenValidationMode` | DecryptOnly | DecryptOnly or DecryptAndValidate |
| `AllowUserMultipleSessions` | true | Allow concurrent sessions |
| `AuthenticationBackend` | Database | Database or LDAP |
| `IsActiveByDefault` | true | New users active by default |
| `IsLockedByDefault` | true | New users locked by default |

## Authentication Backends

### Database (Default)

Credentials stored encrypted in your database:

```csharp
AuthenticationBackend = AuthenticationBackend.Database
```

### LDAP/Active Directory

Authenticate against LDAP/AD server:

```csharp
AuthenticationBackend = AuthenticationBackend.LDAP,
AD_LDAP = "ldap://your-domain.com",
AD_Domain = "your-domain.com",
AD_User = "service-account",
AD_Password = "service-password",
AD_UseSSL = true,
AD_BaseDN = "DC=your-domain,DC=com"
```

Users are automatically imported from AD on first login.

## Blazor UI Components

| Component | Description |
|-----------|-------------|
| `AuthLayout` | Protected page wrapper with user context |
| `LoginPart` | Login form with validation |
| `SignUpPart` | Registration form |
| `Logout` | Logout handler |

### Login Page Example

```razor
@page "/login"
@layout AuthEmptyLayout

<LoginPart OnLoginSuccess="HandleSuccess" />

@code {
    void HandleSuccess() => NavManager.NavigateTo("/dashboard");
}
```

## API Protection

Protect API endpoints using action filters:

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    // Requires authentication
    [HttpGet]
    [IDFAuthUser]
    public IActionResult GetOrders() { }

    // Requires specific permission
    [HttpPost]
    [IDFAuthUserWithPermissions("Order:Create")]
    public IActionResult CreateOrder([FromBody] Order order) { }

    // Requires specific role
    [HttpDelete("{id}")]
    [IDFAuthUserWithRoles("Administrators")]
    public IActionResult DeleteOrder(string id) { }
}
```

## Permission System

Granular entity-action permissions:

```
Entity + Action + Role = Permission

Examples:
- Order:Create - Can create orders
- Order:Edit   - Can edit orders
- Order:Delete - Can delete orders
```

### Grant Permissions

```csharp
// Create permission
IDFManager.permissionService.Create("Order", "Create", roleId);

// Check permission
bool canCreate = IDFManager.userService.ValidateUserPermission(
    userId, "Order", "Create"
);
```

## Service API

### Authentication

```csharp
// Login
var result = IDFManager.authService.AuthUserLogin("username", "password");
if (result.SecurityValidationResult == SecurityValidationResult.Ok)
{
    string token = result.UserToken.Token;
}

// Validate Token
var validation = IDFManager.authService.ValidateUserToken(token);
```

### User Management

```csharp
// Create user
string userId = IDFManager.userService.Create(
    userName: "john.doe",
    password: "SecurePass123",
    fullName: "John Doe",
    email: "john@example.com"
);

// Assign role
IDFManager.userService.GrantRole(userId, "Administrators");

// Lock/unlock
IDFManager.userService.Lock(userId);
IDFManager.userService.Unlock(userId);
```

### Role Management

```csharp
// Create role
string roleId = IDFManager.roleService.Create("Editors");

// Assign permission to role
IDFManager.permissionService.Create("Article", "Edit", roleId);
```

## Security Features

- **Token Encryption**: AES encryption with configurable key
- **Password Hashing**: Per-user private key hashing
- **Cookie Security**: SameSite=Strict, auto-expiration
- **Session Management**: IP tracking, concurrent session control
- **Account Security**: Locking, soft deletes, active/inactive status

## Project Structure

```
CMouss.IdentityFramework/
├── CMouss.IdentityFramework           # Core library
├── CMouss.IdentityFramework.BlazorUI  # Blazor components
├── CMouss.IdentityFramework.API.Serving  # API filters
├── CMouss.IdentityFramework.API.Models   # Shared models
├── CMouss.IdentityFramework.Client    # Client library
├── BlazorUIDemo/                      # Demo application
└── Docs/                              # Documentation
```

## Demo Application

A working Blazor Server demo is included in `BlazorUIDemo/`:

```bash
cd BlazorUIDemo
dotnet run
```

## Documentation

See the [Docs](Docs/) folder for detailed documentation:

- [Getting Started](Docs/Getting-Started.md)
- [Configuration Guide](Docs/Configuration-Guide.md)
- [Authentication & Authorization](Docs/Authentication-Authorization.md)
- [API Reference](Docs/API-Reference.md)
- [Blazor UI Guide](Docs/CMouss.IdentityFramework.BlazorUI.md)

## Requirements

- .NET 8.0 or higher
- Entity Framework Core 9.0+
- SQLite or SQL Server

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**CMouss**

---

If you find this project helpful, please consider giving it a star!
