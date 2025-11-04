# CMouss.IdentityFramework

The core identity framework package providing authentication, authorization, and user management services.

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [IDFManager](#idfmanager)
- [Configuration](#configuration)
- [Data Models](#data-models)
- [Services](#services)
- [Database Context](#database-context)
- [Authentication Backends](#authentication-backends)
- [Code Examples](#code-examples)

## Overview

CMouss.IdentityFramework is the core package that provides:

- User authentication and authorization
- Role-based access control (RBAC)
- Entity-Action permission system
- Token-based authentication
- LDAP/Active Directory integration
- Multi-database support (SQL Server, SQLite)
- User, Role, and Permission management

## Installation

```bash
dotnet add package CMouss.IdentityFramework
```

### Dependencies

- .NET 8.0+
- Entity Framework Core 8.0+
- Microsoft.EntityFrameworkCore.SqlServer (for SQL Server)
- Microsoft.EntityFrameworkCore.Sqlite (for SQLite)
- System.DirectoryServices.Protocols (for LDAP)

## IDFManager

`IDFManager` is the central static class that provides access to all framework services.

### Static Properties

```csharp
// Access to all services
IDFManager.userService
IDFManager.authService
IDFManager.roleService
IDFManager.permissionService
IDFManager.entityService
IDFManager.permissionTypeService
IDFManager.userTokenService
IDFManager.appService
IDFManager.appAccessService

// Configuration
IDFManager.config

// Database context
IDFManager.GetDB() // Returns IDFDBContext instance
```

### Configuration Method

```csharp
IDFManager.Configure(IDFManagerConfig config)
```

Initialize the framework before using any services.

## Configuration

### IDFManagerConfig Class

Complete configuration options:

```csharp
public class IDFManagerConfig
{
    // Database Configuration
    public DatabaseType DatabaseType { get; set; } = DatabaseType.MSSQL;
    public string DBConnectionString { get; set; }
    public DBLifeCycle DBLifeCycle { get; set; } = DBLifeCycle.InMemoryOnly;

    // ID Generation
    public IDGeneratorLevel IDGeneratorLevel { get; set; } = IDGeneratorLevel._128;

    // Administrator Setup
    public string AdministratorRoleName { get; set; } = "Administrators";
    public string AdministratorRoleId { get; set; } = "Administrators";
    public string AdministratorUserName { get; set; } = "Admin";
    public string AdministratorPassword { get; set; } = "Admin";

    // Pagination
    public int DefaultListPageSize { get; set; } = 10;

    // User Defaults
    public bool IsActiveByDefault { get; set; } = true;
    public bool IsLockedByDefault { get; set; } = false;

    // Token Configuration
    public int DefaultTokenLifeTime { get; set; } = 60; // Minutes
    public int DefaultAppAccessLifeTime { get; set; } = 60; // Minutes
    public bool AllowUserMultipleSessions { get; set; } = false;
    public string TokenEncryptionKey { get; set; }
    public TokenValidationMode TokenValidationMode { get; set; } = TokenValidationMode.DecryptOnly;

    // Authentication Backend
    public AuthenticationBackend AuthenticationBackend { get; set; } = AuthenticationBackend.Database;

    // LDAP Configuration
    public string LDAPServerAddress { get; set; }
    public int LDAPServerPort { get; set; } = 389;
    public string LDAPBindDN { get; set; }
    public string LDAPBindPassword { get; set; }
    public string LDAPSearchBase { get; set; }
    public string LDAPUserSearchFilter { get; set; } = "(&(objectClass=user)(sAMAccountName={0}))";
    public string LDAPGroupSearchFilter { get; set; }
    public bool LDAPUseSSL { get; set; } = false;

    // Active Directory Configuration
    public string ADDomain { get; set; }
    public string ADServerAddress { get; set; }
}
```

### Configuration Example

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    // Database settings
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",
    DBLifeCycle = DBLifeCycle.Both,

    // ID generation (32, 64, 96, or 128 character GUIDs)
    IDGeneratorLevel = IDGeneratorLevel._64,

    // Admin account
    AdministratorUserName = "admin",
    AdministratorPassword = "SecurePassword123!",
    AdministratorRoleName = "Administrators",

    // Token settings
    TokenEncryptionKey = "YourSecretKey123456",
    DefaultTokenLifeTime = 120, // 2 hours
    TokenValidationMode = TokenValidationMode.DecryptAndValidate,
    AllowUserMultipleSessions = false,

    // User defaults
    IsActiveByDefault = true,
    IsLockedByDefault = false,

    // Authentication
    AuthenticationBackend = AuthenticationBackend.Database,

    // Pagination
    DefaultListPageSize = 20
});
```

## Data Models

### User

Represents a user in the system.

```csharp
public class User
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; } // Stored hashed
    public string PrivateKey { get; set; } // For encryption
    public string FullName { get; set; }
    public string Email { get; set; }
    public bool IsLocked { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateDate { get; set; }
    public string LastIPAddress { get; set; }

    // Navigation properties
    public List<Role> Roles { get; set; }
    public List<App> Apps { get; set; }
}
```

### Role

Represents a role with associated permissions.

```csharp
public class Role
{
    public string Id { get; set; }
    public string Title { get; set; }

    // Navigation properties
    public List<Permission> Permissions { get; set; }
    public List<User> Users { get; set; }
}
```

### Permission

Entity-Action based permission.

```csharp
public class Permission
{
    public string Id { get; set; }
    public string EntityId { get; set; }
    public string PermissionTypeId { get; set; }
    public string RoleId { get; set; }

    // Navigation properties
    public Entity Entity { get; set; }
    public PermissionType PermissionType { get; set; }
    public Role Role { get; set; }
}
```

### Entity

Represents a resource/entity type (e.g., "User", "Order", "Product").

```csharp
public class Entity
{
    public string Id { get; set; }
    public string Title { get; set; }

    // Navigation properties
    public List<Permission> Permissions { get; set; }
}
```

### PermissionType

Represents an action type (e.g., "Create", "Read", "Update", "Delete").

```csharp
public class PermissionType
{
    public string Id { get; set; }
    public string Title { get; set; }

    // Navigation properties
    public List<Permission> Permissions { get; set; }
}
```

### UserToken

Authentication token for users.

```csharp
public class UserToken
{
    public string Token { get; set; }
    public DateTime ExpireDate { get; set; }
    public string UserId { get; set; }
    public string IPAddress { get; set; }

    // Navigation property
    public User User { get; set; }
}
```

### App

Application registration for multi-tenancy.

```csharp
public class App
{
    public string Id { get; set; }
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public string OwnerId { get; set; }

    // Navigation property
    public User Owner { get; set; }
}
```

## Services

### AuthService

Handles authentication and authorization.

#### Methods

##### AuthUserLogin

Authenticate user with username and password.

```csharp
AuthResult AuthUserLogin(string userName, string password, string ipAddress)
```

Example:
```csharp
var result = IDFManager.authService.AuthUserLogin("john.doe", "password123", "127.0.0.1");

if (result.Status == SecurityValidationResult.Ok)
{
    string token = result.UserToken.Token;
    User user = result.User;
}
```

##### AuthUserToken

Validate a user token.

```csharp
AuthResult AuthUserToken(string token, TokenValidationMode validationMode, string ipAddress)
```

Example:
```csharp
var result = IDFManager.authService.AuthUserToken(
    "encrypted-token",
    TokenValidationMode.DecryptAndValidate,
    "127.0.0.1"
);
```

##### AuthUserTokenWithRole

Validate token and check for specific role.

```csharp
AuthResult AuthUserTokenWithRole(string token, string roleId, string ipAddress)
```

##### AuthUserTokenWithRoles

Validate token and check for any of the specified roles.

```csharp
AuthResult AuthUserTokenWithRoles(string token, List<string> roleIds, string ipAddress)
```

##### AuthUserTokenWithPermission

Validate token and check for specific permission.

```csharp
AuthResult AuthUserTokenWithPermission(
    string token,
    EntityPermission entityPermission,
    string ipAddress
)
```

Example:
```csharp
var result = IDFManager.authService.AuthUserTokenWithPermission(
    token,
    new EntityPermission { EntityId = "Order", PermissionTypeId = "Create" },
    "127.0.0.1"
);
```

##### AuthUserTokenWithPermissions

Validate token and check for multiple permissions (all required).

```csharp
AuthResult AuthUserTokenWithPermissions(
    string token,
    List<EntityPermission> entityPermissions,
    string ipAddress
)
```

##### AuthUserTokenWithPermissionsOrRoles

Validate token and check for permissions OR roles (any match).

```csharp
AuthResult AuthUserTokenWithPermissionsOrRoles(
    string token,
    List<EntityPermission> permissions,
    List<string> roles,
    string ipAddress
)
```

### UserService

User management operations.

#### Methods

##### Create

Create a new user (admin operation).

```csharp
ServiceResult<User> Create(User user)
```

Example:
```csharp
var result = IDFManager.userService.Create(new User
{
    UserName = "john.doe",
    Password = "Password123!",
    FullName = "John Doe",
    Email = "john@example.com",
    IsActive = true,
    IsLocked = false
});

if (result.IsSuccessful)
{
    User createdUser = result.Result;
}
```

##### Register

User self-registration.

```csharp
ServiceResult<User> Register(User user)
```

##### Update

Update user information.

```csharp
ServiceResult<User> Update(User user)
```

##### Delete

Soft delete a user.

```csharp
ServiceResult<User> Delete(string userId)
```

##### Details

Get user details by ID.

```csharp
ServiceResult<User> Details(string userId)
```

##### Search

Search users with pagination.

```csharp
ServiceResult<UserSearchResult> Search(UserSearchModel searchModel)
```

Example:
```csharp
var result = IDFManager.userService.Search(new UserSearchModel
{
    SearchTerm = "john",
    PageNumber = 1,
    PageSize = 10,
    IncludeDeleted = false,
    IncludeLocked = false
});

foreach (var user in result.Result.Users)
{
    Console.WriteLine($"{user.UserName} - {user.FullName}");
}
```

##### Lock / Unlock

```csharp
ServiceResult<User> Lock(string userId)
ServiceResult<User> Unlock(string userId)
```

##### ChangePassword

Admin changes user password.

```csharp
ServiceResult<User> ChangePassword(string userId, string newPassword)
```

##### ChangeMyPassword

User changes their own password.

```csharp
ServiceResult<User> ChangeMyPassword(string userId, string oldPassword, string newPassword)
```

##### GetRoles

Get user's roles.

```csharp
ServiceResult<List<Role>> GetRoles(string userId)
```

##### GrantRole / RevokeRole

```csharp
ServiceResult<User> GrantRole(string userId, string roleId)
ServiceResult<User> RevokeRole(string userId, string roleId)
```

Example:
```csharp
// Grant admin role
IDFManager.userService.GrantRole(userId, "Administrators");

// Revoke role
IDFManager.userService.RevokeRole(userId, "Administrators");
```

##### ValidateUserRole

Check if user has a specific role.

```csharp
bool ValidateUserRole(string userId, string roleId)
```

##### ValidateUserPermission

Check if user has a specific permission.

```csharp
bool ValidateUserPermission(string userId, string entityId, string permissionTypeId)
```

Example:
```csharp
bool canCreateOrders = IDFManager.userService.ValidateUserPermission(
    userId,
    "Order",
    "Create"
);
```

##### ValidateUserRoleOrPermission

Check if user has role OR permission.

```csharp
bool ValidateUserRoleOrPermission(
    string userId,
    string roleId,
    string entityId,
    string permissionTypeId
)
```

##### GetUserPermissions

Get all user permissions.

```csharp
List<Permission> GetUserPermissions(string userId)
```

##### GetUserEntityPermissions

Get user permissions for a specific entity.

```csharp
List<Permission> GetUserEntityPermissions(string userId, string entityId)
```

### RoleService

Role management operations.

#### Methods

```csharp
ServiceResult<Role> Create(Role role)
ServiceResult<Role> Update(Role role)
ServiceResult<Role> Delete(string roleId)
ServiceResult<Role> Details(string roleId)
ServiceResult<List<Role>> GetAll()
ServiceResult<List<Role>> Search(string searchTerm)
```

### PermissionService

Permission management operations.

#### Methods

```csharp
ServiceResult<Permission> Create(Permission permission)
ServiceResult<Permission> Delete(string permissionId)
ServiceResult<List<Permission>> GetRolePermissions(string roleId)
ServiceResult<List<Permission>> GetEntityPermissions(string entityId)
```

### EntityService

Entity (resource) management.

#### Methods

```csharp
ServiceResult<Entity> Create(Entity entity)
ServiceResult<Entity> Update(Entity entity)
ServiceResult<Entity> Delete(string entityId)
ServiceResult<Entity> Details(string entityId)
ServiceResult<List<Entity>> GetAll()
```

Example:
```csharp
// Create entities for your application
IDFManager.entityService.Create(new Entity { Id = "Order", Title = "Order" });
IDFManager.entityService.Create(new Entity { Id = "Product", Title = "Product" });
IDFManager.entityService.Create(new Entity { Id = "Customer", Title = "Customer" });
```

### PermissionTypeService

Permission type (action) management.

#### Methods

```csharp
ServiceResult<PermissionType> Create(PermissionType permissionType)
ServiceResult<PermissionType> Update(PermissionType permissionType)
ServiceResult<PermissionType> Delete(string permissionTypeId)
ServiceResult<PermissionType> Details(string permissionTypeId)
ServiceResult<List<PermissionType>> GetAll()
```

Example:
```csharp
// Create permission types for your application
IDFManager.permissionTypeService.Create(new PermissionType { Id = "Create", Title = "Create" });
IDFManager.permissionTypeService.Create(new PermissionType { Id = "Read", Title = "Read" });
IDFManager.permissionTypeService.Create(new PermissionType { Id = "Update", Title = "Update" });
IDFManager.permissionTypeService.Create(new PermissionType { Id = "Delete", Title = "Delete" });
```

## Database Context

### IDFDBContext

Entity Framework DbContext for the identity framework.

#### DbSets

```csharp
public DbSet<User> Users { get; set; }
public DbSet<Role> Roles { get; set; }
public DbSet<Permission> Permissions { get; set; }
public DbSet<Entity> Entities { get; set; }
public DbSet<PermissionType> PermissionTypes { get; set; }
public DbSet<UserToken> UserTokens { get; set; }
public DbSet<App> Apps { get; set; }
public DbSet<AppAccess> AppAccess { get; set; }
```

#### Methods

##### InsertMasterData

Creates default admin user and role.

```csharp
public void InsertMasterData()
```

Usage:
```csharp
using var db = new IDFDBContext();
db.Database.EnsureCreated();
db.InsertMasterData();
```

#### Database Lifecycle

Configure with `DBLifeCycle` enum:

- **InMemoryOnly**: Single shared instance (best performance)
- **OnRequestOnly**: New instance per request (isolated transactions)
- **Both**: Use in-memory, fallback to new instance

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DBLifeCycle = DBLifeCycle.InMemoryOnly
});
```

## Authentication Backends

### Database Authentication

Default authentication using database-stored credentials.

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    AuthenticationBackend = AuthenticationBackend.Database
});
```

### LDAP Authentication

Authenticate against LDAP/Active Directory server.

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    AuthenticationBackend = AuthenticationBackend.LDAP,
    LDAPServerAddress = "ldap://ldap.company.com",
    LDAPServerPort = 389,
    LDAPBindDN = "CN=Admin,DC=company,DC=com",
    LDAPBindPassword = "password",
    LDAPSearchBase = "DC=company,DC=com",
    LDAPUserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))",
    LDAPUseSSL = false
});
```

### Active Directory Authentication

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    AuthenticationBackend = AuthenticationBackend.LDAP,
    ADDomain = "company.com",
    ADServerAddress = "dc.company.com",
    LDAPBindDN = "CN=Admin,DC=company,DC=com",
    LDAPBindPassword = "admin_password",
    LDAPSearchBase = "DC=company,DC=com",
    LDAPUserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))"
});
```

When LDAP authentication is enabled:
1. User credentials are validated against LDAP
2. User is automatically imported/synced to local database
3. Local user record is used for authorization (roles, permissions)

## Code Examples

### Complete Setup Example

```csharp
using CMouss.IdentityFramework;
using CMouss.IdentityFramework.Models;

// 1. Configure
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",
    AdministratorUserName = "admin",
    AdministratorPassword = "Admin@123",
    TokenEncryptionKey = "MySecretKey",
    DefaultTokenLifeTime = 60
});

// 2. Initialize database
using var db = new IDFDBContext();
db.Database.EnsureCreated();
db.InsertMasterData();

// 3. Create entities and permission types
IDFManager.entityService.Create(new Entity { Id = "Order", Title = "Order Management" });
IDFManager.permissionTypeService.Create(new PermissionType { Id = "Create", Title = "Create" });
IDFManager.permissionTypeService.Create(new PermissionType { Id = "Update", Title = "Update" });

// 4. Create a role with permissions
var roleResult = IDFManager.roleService.Create(new Role
{
    Id = "OrderManager",
    Title = "Order Manager"
});

IDFManager.permissionService.Create(new Permission
{
    RoleId = "OrderManager",
    EntityId = "Order",
    PermissionTypeId = "Create"
});

IDFManager.permissionService.Create(new Permission
{
    RoleId = "OrderManager",
    EntityId = "Order",
    PermissionTypeId = "Update"
});

// 5. Create a user
var userResult = IDFManager.userService.Create(new User
{
    UserName = "john.doe",
    Password = "Password123!",
    FullName = "John Doe",
    Email = "john@example.com"
});

// 6. Grant role to user
IDFManager.userService.GrantRole(userResult.Result.Id, "OrderManager");

// 7. Authenticate
var authResult = IDFManager.authService.AuthUserLogin("john.doe", "Password123!", "127.0.0.1");

if (authResult.Status == SecurityValidationResult.Ok)
{
    string token = authResult.UserToken.Token;

    // 8. Validate with permission
    var permissionCheck = IDFManager.authService.AuthUserTokenWithPermission(
        token,
        new EntityPermission { EntityId = "Order", PermissionTypeId = "Create" },
        "127.0.0.1"
    );

    if (permissionCheck.Status == SecurityValidationResult.Ok)
    {
        Console.WriteLine("User can create orders!");
    }
}
```

### Working with Permissions

```csharp
// Get all user permissions
var permissions = IDFManager.userService.GetUserPermissions(userId);

foreach (var permission in permissions)
{
    Console.WriteLine($"{permission.Entity.Title}:{permission.PermissionType.Title}");
}

// Check specific permission
bool canDelete = IDFManager.userService.ValidateUserPermission(
    userId,
    "Order",
    "Delete"
);

// Get entity-specific permissions
var orderPermissions = IDFManager.userService.GetUserEntityPermissions(userId, "Order");
```

### Token Management

```csharp
// Login and get token
var authResult = IDFManager.authService.AuthUserLogin("username", "password", "127.0.0.1");
string token = authResult.UserToken.Token;

// Later, validate token
var validationResult = IDFManager.authService.AuthUserToken(
    token,
    TokenValidationMode.DecryptAndValidate,
    "127.0.0.1"
);

if (validationResult.Status == SecurityValidationResult.Ok)
{
    User user = validationResult.User;
    // Token is valid, proceed
}
```

## ServiceResult Pattern

All service methods return `ServiceResult<T>`:

```csharp
public class ServiceResult<T>
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; }
    public T Result { get; set; }
}
```

Usage:
```csharp
var result = IDFManager.userService.Create(newUser);

if (result.IsSuccessful)
{
    User createdUser = result.Result;
    Console.WriteLine($"User created: {createdUser.Id}");
}
else
{
    Console.WriteLine($"Error: {result.Message}");
}
```

## Security Validation Results

```csharp
public enum SecurityValidationResult
{
    Ok,
    IncorrectCredentials,
    IncorrectToken,
    UnAuthorized,
    InActiveUser,
    LockedUser,
    DeletedUser,
    ExpiredToken
}
```

---

[← Back to README](README.md) | [Configuration Guide →](Configuration-Guide.md)
