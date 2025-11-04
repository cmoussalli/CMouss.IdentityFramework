# CMouss.IdentityFramework Documentation

Welcome to the CMouss.IdentityFramework documentation. This is a comprehensive identity and access management framework for .NET applications.

## Overview

CMouss.IdentityFramework is a complete authentication and authorization solution that provides:

- **User Management**: Complete CRUD operations for users with role-based access control
- **Role-Based Access Control (RBAC)**: Flexible role management system
- **Entity-Action Permission System**: Granular permission control based on entities and actions
- **Token-Based Authentication**: Secure, encrypted token authentication
- **LDAP/Active Directory Integration**: Support for enterprise authentication
- **Multi-Tenancy Support**: Application-based access control
- **Multiple Database Support**: SQL Server and SQLite
- **Ready-to-Use API Controllers**: RESTful API endpoints out of the box
- **Blazor UI Components**: Pre-built authentication UI for Blazor applications

## Packages

The framework consists of three main NuGet packages:

### 1. CMouss.IdentityFramework
Core identity framework providing authentication, authorization, and user management services.

**Key Features**:
- User, Role, and Permission management
- Token-based authentication
- LDAP/AD integration
- Entity Framework Core database support
- Configurable security policies

[Read Full Documentation →](CMouss.IdentityFramework.md)

### 2. CMouss.IdentityFramework.APIServer
ASP.NET Core API controllers and action filters for REST API implementation.

**Key Features**:
- Pre-built API controllers for authentication and user management
- Authorization action filters
- Role and permission-based endpoint protection
- Standardized API routes

[Read Full Documentation →](CMouss.IdentityFramework.APIServer.md)

### 3. CMouss.IdentityFramework.BlazorUI
Blazor Server components for authentication UI.

**Key Features**:
- Login and registration components
- Authentication layout with session management
- Secure token storage
- Customizable labels and navigation
- Cascading authentication state

[Read Full Documentation →](CMouss.IdentityFramework.BlazorUI.md)

## Quick Start

### Installation

Install the required NuGet packages:

```bash
# Core framework (required)
dotnet add package CMouss.IdentityFramework

# For API projects
dotnet add package CMouss.IdentityFramework.APIServer

# For Blazor Server projects
dotnet add package CMouss.IdentityFramework.BlazorUI
```

### Basic Setup

```csharp
// Configure the identity framework
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",
    AdministratorUserName = "admin",
    AdministratorPassword = "P@ssw0rd123",
    TokenEncryptionKey = "your-secret-key-here"
});

// Initialize the database
using var db = new IDFDBContext();
db.Database.EnsureCreated();
db.InsertMasterData();
```

[Continue to Getting Started Guide →](Getting-Started.md)

## Documentation Structure

- [Getting Started](Getting-Started.md) - Quick start guide and installation
- [Configuration Guide](Configuration-Guide.md) - Detailed configuration options
- [Authentication & Authorization](Authentication-Authorization.md) - Security concepts and implementation
- [API Reference](API-Reference.md) - Complete API documentation

### Package-Specific Documentation

- [CMouss.IdentityFramework](CMouss.IdentityFramework.md) - Core framework documentation
- [CMouss.IdentityFramework.APIServer](CMouss.IdentityFramework.APIServer.md) - API server documentation
- [CMouss.IdentityFramework.BlazorUI](CMouss.IdentityFramework.BlazorUI.md) - Blazor UI documentation

## Architecture

### How It Works

```
┌─────────────────────────────────────────────────────────────┐
│                     Application Layer                        │
├─────────────────────────────────────────────────────────────┤
│  Blazor Components          API Controllers                  │
│  (BlazorUI Package)         (APIServer Package)              │
├─────────────────────────────────────────────────────────────┤
│              Core Framework Services                         │
│  AuthService | UserService | RoleService                     │
│  (IdentityFramework Package)                                 │
├─────────────────────────────────────────────────────────────┤
│              Entity Framework Core                           │
│  IDFDBContext | Migrations                                   │
├─────────────────────────────────────────────────────────────┤
│              Database                                        │
│  SQL Server / SQLite / LDAP                                  │
└─────────────────────────────────────────────────────────────┘
```

### Key Components

- **IDFManager**: Central configuration and service access point
- **Services**: Business logic layer (AuthService, UserService, etc.)
- **Models**: Entity Framework models (User, Role, Permission, etc.)
- **IDFDBContext**: Database context with support for multiple database types
- **API Controllers**: RESTful endpoints for all operations
- **Action Filters**: Authorization attributes for API protection
- **Blazor Components**: UI components with authentication state management

## Use Cases

### 1. Web API with Token Authentication

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    [IDFAuthUser] // Requires valid token
    public IActionResult GetOrders()
    {
        // Protected endpoint
    }

    [HttpPost]
    [IDFAuthUserWithPermissions("Order:Create")] // Requires specific permission
    public IActionResult CreateOrder([FromBody] Order order)
    {
        // Only users with Order:Create permission can access
    }
}
```

### 2. Blazor Server Application

```razor
@page "/dashboard"
@layout AuthLayout

<AuthorizeView>
    <Authorized>
        <h1>Welcome, @authLayoutModel.User.FullName</h1>
    </Authorized>
</AuthorizeView>

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }
}
```

### 3. Direct Service Usage

```csharp
// Authenticate user
var authResult = IDFManager.authService.AuthUserLogin("john.doe", "password", "127.0.0.1");

if (authResult.Status == SecurityValidationResult.Ok)
{
    string token = authResult.UserToken.Token;

    // Validate permission
    bool hasPermission = IDFManager.userService.ValidateUserPermission(
        authResult.UserToken.UserId,
        "Order",
        "Create"
    );
}
```

## Security Features

- **Encrypted Tokens**: All tokens are encrypted using AES encryption
- **Password Hashing**: Secure password storage with hashing
- **Token Expiration**: Configurable token lifetime
- **IP Address Tracking**: Track user sessions by IP
- **Multiple Session Control**: Allow or prevent concurrent sessions
- **Soft Deletes**: Users can be marked as deleted without data loss
- **Account Locking**: Lock/unlock user accounts
- **LDAP Integration**: Enterprise authentication support

## Requirements

- .NET 8.0 or higher
- Entity Framework Core 8.0+
- SQL Server 2016+ or SQLite 3.0+
- (Optional) Active Directory/LDAP server for LDAP authentication

## Support

For issues, questions, or contributions, please visit:
- GitHub Repository: [Your Repository URL]
- Documentation: This directory
- NuGet Packages: [NuGet.org/CMouss.IdentityFramework]

## License

[Your License Here]

## Version

Current Version: 1.1.3

---

**Next Steps**: [Continue to Getting Started Guide →](Getting-Started.md)
