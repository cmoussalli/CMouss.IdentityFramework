# API Reference

Complete API reference for CMouss.IdentityFramework services and methods.

## Table of Contents

- [IDFManager](#idfmanager)
- [AuthService](#authservice)
- [UserService](#userservice)
- [RoleService](#roleservice)
- [PermissionService](#permissionservice)
- [EntityService](#entityservice)
- [PermissionTypeService](#permissiontypeservice)
- [UserTokenService](#usertokenservice)
- [AppService](#appservice)
- [Data Models](#data-models)
- [Enumerations](#enumerations)

## IDFManager

Static manager class providing access to all framework services.

### Static Properties

```csharp
public static class IDFManager
{
    public static AuthService authService { get; }
    public static UserService userService { get; }
    public static RoleService roleService { get; }
    public static PermissionService permissionService { get; }
    public static EntityService entityService { get; }
    public static PermissionTypeService permissionTypeService { get; }
    public static UserTokenService userTokenService { get; }
    public static AppService appService { get; }
    public static AppAccessService appAccessService { get; }

    public static IDFManagerConfig config { get; }
}
```

### Methods

#### Configure

```csharp
public static void Configure(IDFManagerConfig config)
```

Initialize the framework with configuration settings.

**Parameters:**
- `config` - IDFManagerConfig instance with all settings

**Example:**
```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",
    TokenEncryptionKey = "SecretKey123"
});
```

#### GetDB

```csharp
public static IDFDBContext GetDB()
```

Get database context instance based on configured lifecycle.

**Returns:** IDFDBContext instance

## AuthService

Authentication and authorization service.

### Methods

#### AuthUserLogin

```csharp
public AuthResult AuthUserLogin(string userName, string password, string ipAddress)
```

Authenticate user with username and password.

**Parameters:**
- `userName` - Username
- `password` - Password (plain text, will be hashed for comparison)
- `ipAddress` - Client IP address

**Returns:** AuthResult with Status, User, and UserToken

**Example:**
```csharp
var result = IDFManager.authService.AuthUserLogin("john.doe", "password", "192.168.1.1");
if (result.Status == SecurityValidationResult.Ok)
{
    string token = result.UserToken.Token;
}
```

#### AuthUserToken

```csharp
public AuthResult AuthUserToken(string token, TokenValidationMode validationMode, string ipAddress)
```

Validate user token.

**Parameters:**
- `token` - Encrypted token string
- `validationMode` - DecryptOnly or DecryptAndValidate
- `ipAddress` - Client IP address

**Returns:** AuthResult with Status and User

#### AuthUserTokenWithRole

```csharp
public AuthResult AuthUserTokenWithRole(string token, string roleId, string ipAddress)
```

Validate token and check if user has specific role.

**Parameters:**
- `token` - Encrypted token string
- `roleId` - Role ID to check
- `ipAddress` - Client IP address

**Returns:** AuthResult (Status will be UnAuthorized if user lacks role)

#### AuthUserTokenWithRoles

```csharp
public AuthResult AuthUserTokenWithRoles(string token, List<string> roleIds, string ipAddress)
```

Validate token and check if user has any of the specified roles.

**Parameters:**
- `token` - Encrypted token string
- `roleIds` - List of role IDs (user needs ANY one)
- `ipAddress` - Client IP address

**Returns:** AuthResult

#### AuthUserTokenWithPermission

```csharp
public AuthResult AuthUserTokenWithPermission(
    string token,
    EntityPermission entityPermission,
    string ipAddress
)
```

Validate token and check if user has specific permission.

**Parameters:**
- `token` - Encrypted token string
- `entityPermission` - EntityPermission object with EntityId and PermissionTypeId
- `ipAddress` - Client IP address

**Returns:** AuthResult

**Example:**
```csharp
var result = IDFManager.authService.AuthUserTokenWithPermission(
    token,
    new EntityPermission { EntityId = "Order", PermissionTypeId = "Create" },
    "192.168.1.1"
);
```

#### AuthUserTokenWithPermissions

```csharp
public AuthResult AuthUserTokenWithPermissions(
    string token,
    List<EntityPermission> entityPermissions,
    string ipAddress
)
```

Validate token and check if user has all specified permissions.

**Parameters:**
- `token` - Encrypted token string
- `entityPermissions` - List of EntityPermission (user needs ALL)
- `ipAddress` - Client IP address

**Returns:** AuthResult

#### AuthUserTokenWithPermissionsOrRoles

```csharp
public AuthResult AuthUserTokenWithPermissionsOrRoles(
    string token,
    List<EntityPermission> permissions,
    List<string> roles,
    string ipAddress
)
```

Validate token and check if user has any of the permissions OR roles.

**Parameters:**
- `token` - Encrypted token string
- `permissions` - List of EntityPermission
- `roles` - List of role IDs
- `ipAddress` - Client IP address

**Returns:** AuthResult (Success if user has ANY permission or role)

#### AuthAppLogin

```csharp
public AuthResult AuthAppLogin(string appId, string appSecret, string ipAddress)
```

Authenticate application with app credentials.

**Parameters:**
- `appId` - Application ID
- `appSecret` - Application secret
- `ipAddress` - Client IP address

**Returns:** AuthResult with AppAccess token

#### AuthAppAccess

```csharp
public AuthResult AuthAppAccess(string accessToken, string ipAddress)
```

Validate app access token.

**Parameters:**
- `accessToken` - App access token
- `ipAddress` - Client IP address

**Returns:** AuthResult

## UserService

User management service.

### Methods

#### Create

```csharp
public ServiceResult<User> Create(User user)
```

Create new user (admin operation).

**Parameters:**
- `user` - User object with all properties

**Returns:** ServiceResult with created User

**Example:**
```csharp
var result = IDFManager.userService.Create(new User
{
    UserName = "john.doe",
    Password = "Password123!",
    FullName = "John Doe",
    Email = "john@example.com"
});

if (result.IsSuccessful)
{
    User createdUser = result.Result;
}
```

#### Register

```csharp
public ServiceResult<User> Register(User user)
```

User self-registration.

**Parameters:**
- `user` - User object

**Returns:** ServiceResult with registered User

#### Update

```csharp
public ServiceResult<User> Update(User user)
```

Update user information.

**Parameters:**
- `user` - User object with updated properties

**Returns:** ServiceResult with updated User

#### Delete

```csharp
public ServiceResult<User> Delete(string userId)
```

Soft delete user (sets IsDeleted = true).

**Parameters:**
- `userId` - User ID

**Returns:** ServiceResult with deleted User

#### Details

```csharp
public ServiceResult<User> Details(string userId)
```

Get user details by ID.

**Parameters:**
- `userId` - User ID

**Returns:** ServiceResult with User object

#### Search

```csharp
public ServiceResult<UserSearchResult> Search(UserSearchModel searchModel)
```

Search users with pagination.

**Parameters:**
- `searchModel` - UserSearchModel with search criteria

**Returns:** ServiceResult with UserSearchResult (contains list and pagination info)

**Example:**
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
    Console.WriteLine(user.UserName);
}
```

#### Lock

```csharp
public ServiceResult<User> Lock(string userId)
```

Lock user account (sets IsLocked = true).

**Parameters:**
- `userId` - User ID

**Returns:** ServiceResult with locked User

#### Unlock

```csharp
public ServiceResult<User> Unlock(string userId)
```

Unlock user account (sets IsLocked = false).

**Parameters:**
- `userId` - User ID

**Returns:** ServiceResult with unlocked User

#### ChangePassword

```csharp
public ServiceResult<User> ChangePassword(string userId, string newPassword)
```

Admin changes user password.

**Parameters:**
- `userId` - User ID
- `newPassword` - New password (plain text, will be hashed)

**Returns:** ServiceResult with User

#### ChangeMyPassword

```csharp
public ServiceResult<User> ChangeMyPassword(string userId, string oldPassword, string newPassword)
```

User changes their own password.

**Parameters:**
- `userId` - User ID
- `oldPassword` - Current password
- `newPassword` - New password

**Returns:** ServiceResult with User

#### GetRoles

```csharp
public ServiceResult<List<Role>> GetRoles(string userId)
```

Get all roles assigned to user.

**Parameters:**
- `userId` - User ID

**Returns:** ServiceResult with List of Role

#### GrantRole

```csharp
public ServiceResult<User> GrantRole(string userId, string roleId)
```

Grant role to user.

**Parameters:**
- `userId` - User ID
- `roleId` - Role ID

**Returns:** ServiceResult with User

#### RevokeRole

```csharp
public ServiceResult<User> RevokeRole(string userId, string roleId)
```

Revoke role from user.

**Parameters:**
- `userId` - User ID
- `roleId` - Role ID

**Returns:** ServiceResult with User

#### ValidateUserRole

```csharp
public bool ValidateUserRole(string userId, string roleId)
```

Check if user has specific role.

**Parameters:**
- `userId` - User ID
- `roleId` - Role ID

**Returns:** true if user has role, false otherwise

#### ValidateUserPermission

```csharp
public bool ValidateUserPermission(string userId, string entityId, string permissionTypeId)
```

Check if user has specific permission.

**Parameters:**
- `userId` - User ID
- `entityId` - Entity ID
- `permissionTypeId` - Permission Type ID

**Returns:** true if user has permission, false otherwise

#### ValidateUserRoleOrPermission

```csharp
public bool ValidateUserRoleOrPermission(
    string userId,
    string roleId,
    string entityId,
    string permissionTypeId
)
```

Check if user has role OR permission.

**Parameters:**
- `userId` - User ID
- `roleId` - Role ID
- `entityId` - Entity ID
- `permissionTypeId` - Permission Type ID

**Returns:** true if user has role or permission

#### GetUserPermissions

```csharp
public List<Permission> GetUserPermissions(string userId)
```

Get all permissions for user (through their roles).

**Parameters:**
- `userId` - User ID

**Returns:** List of Permission

#### GetUserEntityPermissions

```csharp
public List<Permission> GetUserEntityPermissions(string userId, string entityId)
```

Get user permissions for specific entity.

**Parameters:**
- `userId` - User ID
- `entityId` - Entity ID

**Returns:** List of Permission for the entity

## RoleService

Role management service.

### Methods

#### Create

```csharp
public ServiceResult<Role> Create(Role role)
```

Create new role.

**Parameters:**
- `role` - Role object

**Returns:** ServiceResult with created Role

#### Update

```csharp
public ServiceResult<Role> Update(Role role)
```

Update role.

**Parameters:**
- `role` - Role object with updated properties

**Returns:** ServiceResult with updated Role

#### Delete

```csharp
public ServiceResult<Role> Delete(string roleId)
```

Delete role.

**Parameters:**
- `roleId` - Role ID

**Returns:** ServiceResult

#### Details

```csharp
public ServiceResult<Role> Details(string roleId)
```

Get role details.

**Parameters:**
- `roleId` - Role ID

**Returns:** ServiceResult with Role

#### GetAll

```csharp
public ServiceResult<List<Role>> GetAll()
```

Get all roles.

**Returns:** ServiceResult with List of Role

#### Search

```csharp
public ServiceResult<List<Role>> Search(string searchTerm)
```

Search roles by title.

**Parameters:**
- `searchTerm` - Search text

**Returns:** ServiceResult with List of Role

## PermissionService

Permission management service.

### Methods

#### Create

```csharp
public ServiceResult<Permission> Create(Permission permission)
```

Create new permission.

**Parameters:**
- `permission` - Permission object with RoleId, EntityId, PermissionTypeId

**Returns:** ServiceResult with created Permission

**Example:**
```csharp
var result = IDFManager.permissionService.Create(new Permission
{
    RoleId = "Administrators",
    EntityId = "Order",
    PermissionTypeId = "Create"
});
```

#### Delete

```csharp
public ServiceResult<Permission> Delete(string permissionId)
```

Delete permission.

**Parameters:**
- `permissionId` - Permission ID

**Returns:** ServiceResult

#### GetRolePermissions

```csharp
public ServiceResult<List<Permission>> GetRolePermissions(string roleId)
```

Get all permissions for a role.

**Parameters:**
- `roleId` - Role ID

**Returns:** ServiceResult with List of Permission

#### GetEntityPermissions

```csharp
public ServiceResult<List<Permission>> GetEntityPermissions(string entityId)
```

Get all permissions for an entity.

**Parameters:**
- `entityId` - Entity ID

**Returns:** ServiceResult with List of Permission

## EntityService

Entity (resource) management service.

### Methods

#### Create

```csharp
public ServiceResult<Entity> Create(Entity entity)
```

Create new entity.

**Parameters:**
- `entity` - Entity object

**Returns:** ServiceResult with created Entity

**Example:**
```csharp
IDFManager.entityService.Create(new Entity
{
    Id = "Order",
    Title = "Order Management"
});
```

#### Update

```csharp
public ServiceResult<Entity> Update(Entity entity)
```

Update entity.

**Parameters:**
- `entity` - Entity object

**Returns:** ServiceResult with updated Entity

#### Delete

```csharp
public ServiceResult<Entity> Delete(string entityId)
```

Delete entity.

**Parameters:**
- `entityId` - Entity ID

**Returns:** ServiceResult

#### Details

```csharp
public ServiceResult<Entity> Details(string entityId)
```

Get entity details.

**Parameters:**
- `entityId` - Entity ID

**Returns:** ServiceResult with Entity

#### GetAll

```csharp
public ServiceResult<List<Entity>> GetAll()
```

Get all entities.

**Returns:** ServiceResult with List of Entity

## PermissionTypeService

Permission type (action) management service.

### Methods

#### Create

```csharp
public ServiceResult<PermissionType> Create(PermissionType permissionType)
```

Create new permission type.

**Parameters:**
- `permissionType` - PermissionType object

**Returns:** ServiceResult with created PermissionType

**Example:**
```csharp
IDFManager.permissionTypeService.Create(new PermissionType
{
    Id = "Create",
    Title = "Create"
});
```

#### Update

```csharp
public ServiceResult<PermissionType> Update(PermissionType permissionType)
```

Update permission type.

**Parameters:**
- `permissionType` - PermissionType object

**Returns:** ServiceResult with updated PermissionType

#### Delete

```csharp
public ServiceResult<PermissionType> Delete(string permissionTypeId)
```

Delete permission type.

**Parameters:**
- `permissionTypeId` - Permission Type ID

**Returns:** ServiceResult

#### Details

```csharp
public ServiceResult<PermissionType> Details(string permissionTypeId)
```

Get permission type details.

**Parameters:**
- `permissionTypeId` - Permission Type ID

**Returns:** ServiceResult with PermissionType

#### GetAll

```csharp
public ServiceResult<List<PermissionType>> GetAll()
```

Get all permission types.

**Returns:** ServiceResult with List of PermissionType

## UserTokenService

User token management service.

### Methods

#### CreateToken

```csharp
public ServiceResult<UserToken> CreateToken(string userId, string ipAddress)
```

Create new token for user.

**Parameters:**
- `userId` - User ID
- `ipAddress` - Client IP address

**Returns:** ServiceResult with UserToken

#### ValidateToken

```csharp
public ServiceResult<UserToken> ValidateToken(string token)
```

Validate token (decrypt and check database).

**Parameters:**
- `token` - Encrypted token string

**Returns:** ServiceResult with UserToken

#### DeleteToken

```csharp
public ServiceResult DeleteToken(string token)
```

Delete (revoke) token.

**Parameters:**
- `token` - Token to revoke

**Returns:** ServiceResult

#### GetUserTokens

```csharp
public ServiceResult<List<UserToken>> GetUserTokens(string userId)
```

Get all active tokens for user.

**Parameters:**
- `userId` - User ID

**Returns:** ServiceResult with List of UserToken

## AppService

Application management service.

### Methods

#### Create

```csharp
public ServiceResult<App> Create(App app)
```

Create new application.

**Parameters:**
- `app` - App object

**Returns:** ServiceResult with created App

#### Update

```csharp
public ServiceResult<App> Update(App app)
```

Update application.

**Parameters:**
- `app` - App object

**Returns:** ServiceResult with updated App

#### Delete

```csharp
public ServiceResult<App> Delete(string appId)
```

Delete application.

**Parameters:**
- `appId` - App ID

**Returns:** ServiceResult

#### Details

```csharp
public ServiceResult<App> Details(string appId)
```

Get app details.

**Parameters:**
- `appId` - App ID

**Returns:** ServiceResult with App

#### GetAll

```csharp
public ServiceResult<List<App>> GetAll()
```

Get all applications.

**Returns:** ServiceResult with List of App

#### Search

```csharp
public ServiceResult<List<App>> Search(string searchTerm)
```

Search applications.

**Parameters:**
- `searchTerm` - Search text

**Returns:** ServiceResult with List of App

#### GetUserApps

```csharp
public ServiceResult<List<App>> GetUserApps(string userId)
```

Get all apps owned by user.

**Parameters:**
- `userId` - User ID

**Returns:** ServiceResult with List of App

## Data Models

### User

```csharp
public class User
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string PrivateKey { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public bool IsLocked { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateDate { get; set; }
    public string LastIPAddress { get; set; }

    public List<Role> Roles { get; set; }
    public List<App> Apps { get; set; }
}
```

### Role

```csharp
public class Role
{
    public string Id { get; set; }
    public string Title { get; set; }

    public List<Permission> Permissions { get; set; }
    public List<User> Users { get; set; }
}
```

### Permission

```csharp
public class Permission
{
    public string Id { get; set; }
    public string EntityId { get; set; }
    public string PermissionTypeId { get; set; }
    public string RoleId { get; set; }

    public Entity Entity { get; set; }
    public PermissionType PermissionType { get; set; }
    public Role Role { get; set; }
}
```

### Entity

```csharp
public class Entity
{
    public string Id { get; set; }
    public string Title { get; set; }

    public List<Permission> Permissions { get; set; }
}
```

### PermissionType

```csharp
public class PermissionType
{
    public string Id { get; set; }
    public string Title { get; set; }

    public List<Permission> Permissions { get; set; }
}
```

### UserToken

```csharp
public class UserToken
{
    public string Token { get; set; }
    public DateTime ExpireDate { get; set; }
    public string UserId { get; set; }
    public string IPAddress { get; set; }

    public User User { get; set; }
}
```

### App

```csharp
public class App
{
    public string Id { get; set; }
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public string OwnerId { get; set; }

    public User Owner { get; set; }
}
```

### ServiceResult<T>

```csharp
public class ServiceResult<T>
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; }
    public T Result { get; set; }
}
```

### AuthResult

```csharp
public class AuthResult
{
    public SecurityValidationResult Status { get; set; }
    public IDFAuthenticationMode AuthenticationMode { get; set; }
    public UserToken UserToken { get; set; }
    public AppAccess AppAccess { get; set; }
    public User User { get; set; }
    public App App { get; set; }
}
```

### EntityPermission

```csharp
public class EntityPermission
{
    public string EntityId { get; set; }
    public string PermissionTypeId { get; set; }
}
```

### UserSearchModel

```csharp
public class UserSearchModel
{
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeDeleted { get; set; } = false;
    public bool IncludeLocked { get; set; } = false;
}
```

## Enumerations

### SecurityValidationResult

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

### DatabaseType

```csharp
public enum DatabaseType
{
    MSSQL,
    SQLite
}
```

### TokenValidationMode

```csharp
public enum TokenValidationMode
{
    DecryptOnly,
    DecryptAndValidate
}
```

### AuthenticationBackend

```csharp
public enum AuthenticationBackend
{
    Database,
    LDAP
}
```

### IDGeneratorLevel

```csharp
public enum IDGeneratorLevel
{
    _32,   // 32 characters
    _64,   // 64 characters
    _96,   // 96 characters
    _128   // 128 characters
}
```

### DBLifeCycle

```csharp
public enum DBLifeCycle
{
    InMemoryOnly,
    OnRequestOnly,
    Both
}
```

### IDFAuthenticationMode

```csharp
public enum IDFAuthenticationMode
{
    User,
    App
}
```

---

[← Back to README](README.md) | [Getting Started →](Getting-Started.md)
