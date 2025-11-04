# Authentication & Authorization Guide

Complete guide to implementing authentication and authorization with CMouss.IdentityFramework.

## Table of Contents

- [Overview](#overview)
- [Authentication](#authentication)
- [Authorization](#authorization)
- [Entity-Action Permission System](#entity-action-permission-system)
- [Role-Based Access Control](#role-based-access-control)
- [Token Management](#token-management)
- [Implementation Patterns](#implementation-patterns)
- [Security Considerations](#security-considerations)

## Overview

CMouss.IdentityFramework provides a flexible authentication and authorization system with:

- **Authentication**: Verify user identity (Who are you?)
- **Authorization**: Verify user permissions (What can you do?)
- **Token-Based**: Stateless authentication using encrypted tokens
- **Entity-Action Permissions**: Granular permission control
- **Role-Based Access**: Traditional role-based authorization
- **Hybrid Approach**: Combine roles and permissions

## Authentication

Authentication is the process of verifying user identity.

### User Login

```csharp
var authResult = IDFManager.authService.AuthUserLogin(
    userName: "john.doe",
    password: "Password123!",
    ipAddress: "192.168.1.100"
);

if (authResult.Status == SecurityValidationResult.Ok)
{
    // Authentication successful
    string token = authResult.UserToken.Token;
    User user = authResult.User;
    DateTime expires = authResult.UserToken.ExpireDate;
}
else
{
    // Authentication failed
    switch (authResult.Status)
    {
        case SecurityValidationResult.IncorrectCredentials:
            Console.WriteLine("Invalid username or password");
            break;
        case SecurityValidationResult.InActiveUser:
            Console.WriteLine("Account is inactive");
            break;
        case SecurityValidationResult.LockedUser:
            Console.WriteLine("Account is locked");
            break;
        case SecurityValidationResult.DeletedUser:
            Console.WriteLine("Account has been deleted");
            break;
    }
}
```

### Token Validation

#### Fast Validation (DecryptOnly)

```csharp
var authResult = IDFManager.authService.AuthUserToken(
    token: "encrypted-token-string",
    validationMode: TokenValidationMode.DecryptOnly,
    ipAddress: "192.168.1.100"
);

if (authResult.Status == SecurityValidationResult.Ok)
{
    User user = authResult.User;
    // Token is valid
}
```

#### Secure Validation (DecryptAndValidate)

```csharp
var authResult = IDFManager.authService.AuthUserToken(
    token: "encrypted-token-string",
    validationMode: TokenValidationMode.DecryptAndValidate,
    ipAddress: "192.168.1.100"
);
```

### Authentication Status Results

```csharp
public enum SecurityValidationResult
{
    Ok,                      // Authentication successful
    IncorrectCredentials,    // Wrong username or password
    IncorrectToken,          // Invalid or malformed token
    UnAuthorized,            // Valid token but insufficient permissions
    InActiveUser,            // User account is inactive
    LockedUser,              // User account is locked
    DeletedUser,             // User account is deleted
    ExpiredToken            // Token has expired
}
```

### LDAP/AD Authentication

When configured for LDAP authentication:

```csharp
// Configuration
IDFManager.Configure(new IDFManagerConfig
{
    AuthenticationBackend = AuthenticationBackend.LDAP,
    LDAPServerAddress = "ldap://ldap.company.com",
    LDAPBindDN = "CN=Admin,DC=company,DC=com",
    LDAPBindPassword = "password",
    LDAPSearchBase = "DC=company,DC=com"
});

// Login (same API)
var authResult = IDFManager.authService.AuthUserLogin(
    userName: "john.doe",  // AD username
    password: "ADPassword",
    ipAddress: "192.168.1.100"
);

// User is validated against LDAP and synced to local database
if (authResult.Status == SecurityValidationResult.Ok)
{
    // User authenticated via LDAP
    // User info synced to local database
    // Local roles and permissions apply
}
```

## Authorization

Authorization determines what authenticated users can access.

### Authorization Approaches

1. **Role-Based**: Check if user has specific role(s)
2. **Permission-Based**: Check if user has specific permission(s)
3. **Hybrid**: Check for roles OR permissions

### Role-Based Authorization

```csharp
// Check single role
var authResult = IDFManager.authService.AuthUserTokenWithRole(
    token: "user-token",
    roleId: "Administrators",
    ipAddress: "192.168.1.100"
);

if (authResult.Status == SecurityValidationResult.Ok)
{
    // User has the Administrators role
}
else if (authResult.Status == SecurityValidationResult.UnAuthorized)
{
    // User is authenticated but doesn't have the role
}
```

```csharp
// Check multiple roles (user needs ANY one)
var authResult = IDFManager.authService.AuthUserTokenWithRoles(
    token: "user-token",
    roleIds: new List<string> { "Administrators", "PowerUsers" },
    ipAddress: "192.168.1.100"
);
```

### Permission-Based Authorization

```csharp
// Check single permission
var authResult = IDFManager.authService.AuthUserTokenWithPermission(
    token: "user-token",
    entityPermission: new EntityPermission
    {
        EntityId = "Order",
        PermissionTypeId = "Create"
    },
    ipAddress: "192.168.1.100"
);

if (authResult.Status == SecurityValidationResult.Ok)
{
    // User can create orders
}
```

```csharp
// Check multiple permissions (user needs ALL)
var authResult = IDFManager.authService.AuthUserTokenWithPermissions(
    token: "user-token",
    entityPermissions: new List<EntityPermission>
    {
        new() { EntityId = "Order", PermissionTypeId = "Create" },
        new() { EntityId = "Order", PermissionTypeId = "Update" }
    },
    ipAddress: "192.168.1.100"
);
```

### Hybrid Authorization

Combine roles and permissions (user needs ANY match):

```csharp
var authResult = IDFManager.authService.AuthUserTokenWithPermissionsOrRoles(
    token: "user-token",
    permissions: new List<EntityPermission>
    {
        new() { EntityId = "Order", PermissionTypeId = "Delete" }
    },
    roles: new List<string> { "Administrators" },
    ipAddress: "192.168.1.100"
);

// Success if user has:
// - Administrators role OR
// - Order:Delete permission
```

### Direct User Authorization

Without tokens, using user ID:

```csharp
// Check if user has role
bool hasRole = IDFManager.userService.ValidateUserRole(
    userId: "user-123",
    roleId: "Administrators"
);

// Check if user has permission
bool hasPermission = IDFManager.userService.ValidateUserPermission(
    userId: "user-123",
    entityId: "Order",
    permissionTypeId: "Create"
);

// Check role OR permission
bool hasAccess = IDFManager.userService.ValidateUserRoleOrPermission(
    userId: "user-123",
    roleId: "Administrators",
    entityId: "Order",
    permissionTypeId: "Create"
);
```

## Entity-Action Permission System

The framework uses an Entity-Action permission model for fine-grained access control.

### Concepts

- **Entity**: A resource type (e.g., Order, Product, User, Report)
- **PermissionType**: An action (e.g., Create, Read, Update, Delete, Export)
- **Permission**: Combination of Entity + PermissionType assigned to a Role

### Setting Up Permissions

#### 1. Create Entities

```csharp
// Define resources in your application
IDFManager.entityService.Create(new Entity
{
    Id = "Order",
    Title = "Order Management"
});

IDFManager.entityService.Create(new Entity
{
    Id = "Product",
    Title = "Product Catalog"
});

IDFManager.entityService.Create(new Entity
{
    Id = "Customer",
    Title = "Customer Management"
});

IDFManager.entityService.Create(new Entity
{
    Id = "Report",
    Title = "Reports"
});
```

#### 2. Create Permission Types

```csharp
// Define actions in your application
IDFManager.permissionTypeService.Create(new PermissionType
{
    Id = "Create",
    Title = "Create"
});

IDFManager.permissionTypeService.Create(new PermissionType
{
    Id = "Read",
    Title = "Read/View"
});

IDFManager.permissionTypeService.Create(new PermissionType
{
    Id = "Update",
    Title = "Update/Edit"
});

IDFManager.permissionTypeService.Create(new PermissionType
{
    Id = "Delete",
    Title = "Delete"
});

IDFManager.permissionTypeService.Create(new PermissionType
{
    Id = "Export",
    Title = "Export"
});
```

#### 3. Create Roles

```csharp
IDFManager.roleService.Create(new Role
{
    Id = "OrderManager",
    Title = "Order Manager"
});

IDFManager.roleService.Create(new Role
{
    Id = "ProductManager",
    Title = "Product Manager"
});

IDFManager.roleService.Create(new Role
{
    Id = "ReadOnly",
    Title = "Read Only User"
});
```

#### 4. Assign Permissions to Roles

```csharp
// Order Manager can Create, Read, Update orders
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
    PermissionTypeId = "Read"
});

IDFManager.permissionService.Create(new Permission
{
    RoleId = "OrderManager",
    EntityId = "Order",
    PermissionTypeId = "Update"
});

// Product Manager has full access to products
var productPermissionTypes = new[] { "Create", "Read", "Update", "Delete" };
foreach (var permType in productPermissionTypes)
{
    IDFManager.permissionService.Create(new Permission
    {
        RoleId = "ProductManager",
        EntityId = "Product",
        PermissionTypeId = permType
    });
}

// ReadOnly user can only read
var readOnlyEntities = new[] { "Order", "Product", "Customer" };
foreach (var entity in readOnlyEntities)
{
    IDFManager.permissionService.Create(new Permission
    {
        RoleId = "ReadOnly",
        EntityId = entity,
        PermissionTypeId = "Read"
    });
}
```

#### 5. Assign Roles to Users

```csharp
IDFManager.userService.GrantRole(
    userId: "user-123",
    roleId: "OrderManager"
);
```

### Using Permissions

#### In Business Logic

```csharp
public class OrderService
{
    public ServiceResult<Order> CreateOrder(string userId, Order order)
    {
        // Check permission before operation
        bool canCreate = IDFManager.userService.ValidateUserPermission(
            userId,
            "Order",
            "Create"
        );

        if (!canCreate)
        {
            return ServiceResult<Order>.Failure("You don't have permission to create orders");
        }

        // Proceed with creating order
        // ...
    }

    public ServiceResult DeleteOrder(string userId, string orderId)
    {
        // Only admins or users with Delete permission
        bool canDelete = IDFManager.userService.ValidateUserRoleOrPermission(
            userId,
            "Administrators",
            "Order",
            "Delete"
        );

        if (!canDelete)
        {
            return ServiceResult.Failure("Insufficient permissions to delete orders");
        }

        // Proceed with deletion
        // ...
    }
}
```

#### In API Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    [IDFAuthUserWithPermissions("Order:Read")]
    public IActionResult GetOrders()
    {
        // Only users with Order:Read permission
        return Ok(orders);
    }

    [HttpPost]
    [IDFAuthUserWithPermissions("Order:Create")]
    public IActionResult CreateOrder([FromBody] Order order)
    {
        // Only users with Order:Create permission
        return Ok(createdOrder);
    }

    [HttpPut("{id}")]
    [IDFAuthUserWithPermissions("Order:Update")]
    public IActionResult UpdateOrder(string id, [FromBody] Order order)
    {
        // Only users with Order:Update permission
        return Ok(updatedOrder);
    }

    [HttpDelete("{id}")]
    [IDFAuthUserWithRolesOrPermissions("Administrators", "Order:Delete")]
    public IActionResult DeleteOrder(string id)
    {
        // Administrators OR users with Order:Delete permission
        return Ok();
    }
}
```

#### In Blazor Components

```razor
@page "/orders"
@layout AuthLayout

<h1>Orders</h1>

@if (canCreate)
{
    <button @onclick="ShowCreateForm">Create New Order</button>
}

<table>
    @foreach (var order in orders)
    {
        <tr>
            <td>@order.Id</td>
            <td>@order.Total</td>
            <td>
                @if (canUpdate)
                {
                    <button @onclick="() => EditOrder(order.Id)">Edit</button>
                }
                @if (canDelete)
                {
                    <button @onclick="() => DeleteOrder(order.Id)">Delete</button>
                }
            </td>
        </tr>
    }
</table>

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }

    private List<Order> orders = new();
    private bool canCreate = false;
    private bool canUpdate = false;
    private bool canDelete = false;

    protected override void OnInitialized()
    {
        var userId = authLayoutModel.User.Id;

        canCreate = IDFManager.userService.ValidateUserPermission(userId, "Order", "Create");
        canUpdate = IDFManager.userService.ValidateUserPermission(userId, "Order", "Update");
        canDelete = IDFManager.userService.ValidateUserPermission(userId, "Order", "Delete");

        LoadOrders();
    }
}
```

## Role-Based Access Control

Traditional role-based authorization for broader access control.

### Setting Up Roles

```csharp
// Create roles
IDFManager.roleService.Create(new Role
{
    Id = "Administrators",
    Title = "System Administrators"
});

IDFManager.roleService.Create(new Role
{
    Id = "Managers",
    Title = "Department Managers"
});

IDFManager.roleService.Create(new Role
{
    Id = "Users",
    Title = "Standard Users"
});
```

### Assigning Roles

```csharp
// Grant role to user
IDFManager.userService.GrantRole("user-123", "Managers");

// User can have multiple roles
IDFManager.userService.GrantRole("user-123", "Users");

// Revoke role
IDFManager.userService.RevokeRole("user-123", "Managers");

// Get user's roles
var rolesResult = IDFManager.userService.GetRoles("user-123");
foreach (var role in rolesResult.Result)
{
    Console.WriteLine(role.Title);
}
```

### Using Roles

#### In Code

```csharp
// Check if user has admin role
bool isAdmin = IDFManager.userService.ValidateUserRole(
    userId: "user-123",
    roleId: "Administrators"
);

if (isAdmin)
{
    // Allow admin operation
}
```

#### In API Controllers

```csharp
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    [HttpGet("users")]
    [IDFAuthUserWithRoles("Administrators")]
    public IActionResult GetAllUsers()
    {
        // Only admins can access
        return Ok(users);
    }

    [HttpPost("settings")]
    [IDFAuthUserWithRoles("Administrators,Managers")]
    public IActionResult UpdateSettings([FromBody] Settings settings)
    {
        // Admins or Managers can access
        return Ok();
    }
}
```

#### In Blazor

```razor
@code {
    private bool isAdmin = false;

    protected override void OnInitialized()
    {
        isAdmin = IDFManager.userService.ValidateUserRole(
            authLayoutModel.User.Id,
            "Administrators"
        );
    }
}

@if (isAdmin)
{
    <div class="admin-panel">
        <!-- Admin-only content -->
    </div>
}
```

## Token Management

### Token Lifecycle

1. **Creation**: Token created on successful login
2. **Storage**: Token stored securely (client-side)
3. **Transmission**: Token sent with each request
4. **Validation**: Token validated on server
5. **Expiration**: Token expires after configured lifetime
6. **Renewal**: User re-authenticates when token expires

### Token Storage

#### In Web API Clients

```csharp
// Store token after login
string token = authResult.UserToken.Token;
DateTime expires = authResult.UserToken.ExpireDate;

// Save to secure storage (e.g., keychain, credential manager)
await SecureStorage.SetAsync("user_token", token);
```

#### In Blazor (ProtectedLocalStorage)

```razor
@inject ProtectedLocalStorage localStorage

@code {
    private async Task LoginAsync()
    {
        var authResult = IDFManager.authService.AuthUserLogin(username, password, ipAddress);

        if (authResult.Status == SecurityValidationResult.Ok)
        {
            // Store token securely
            await localStorage.SetAsync("userToken", authResult.UserToken.Token);
            await localStorage.SetAsync("userId", authResult.User.Id);
        }
    }

    private async Task<string> GetTokenAsync()
    {
        var tokenResult = await localStorage.GetAsync<string>("userToken");
        return tokenResult.Success ? tokenResult.Value : null;
    }
}
```

#### In JavaScript/SPA

```javascript
// Store in sessionStorage (cleared on tab close)
sessionStorage.setItem('userToken', token);

// Or localStorage (persists across sessions)
localStorage.setItem('userToken', token);

// Include in API requests
fetch('/api/orders', {
    headers: {
        'userToken': sessionStorage.getItem('userToken')
    }
});
```

### Token Expiration Handling

```csharp
var authResult = IDFManager.authService.AuthUserToken(token, mode, ipAddress);

if (authResult.Status == SecurityValidationResult.ExpiredToken)
{
    // Token expired, redirect to login
    navigationManager.NavigateTo("/login?returnUrl=" + currentUrl);
}
```

### Multiple Sessions

```csharp
// Configuration
IDFManager.Configure(new IDFManagerConfig
{
    AllowUserMultipleSessions = true  // or false
});

// If false:
// - New login invalidates previous tokens
// - User can only be logged in from one device/browser
//
// If true:
// - Multiple tokens can be active simultaneously
// - User can be logged in from multiple devices
```

## Implementation Patterns

### Pattern 1: API with Role-Based Auth

```csharp
[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    [HttpGet]
    [IDFAuthUser]
    public IActionResult GetDocuments()
    {
        // Any authenticated user
        return Ok(documents);
    }

    [HttpPost]
    [IDFAuthUserWithRoles("Authors,Editors")]
    public IActionResult CreateDocument([FromBody] Document doc)
    {
        // Only Authors or Editors
        return Ok(created);
    }

    [HttpDelete("{id}")]
    [IDFAuthUserWithRoles("Administrators")]
    public IActionResult DeleteDocument(string id)
    {
        // Only Administrators
        return Ok();
    }
}
```

### Pattern 2: Business Layer with Permissions

```csharp
public class DocumentService
{
    public ServiceResult<Document> CreateDocument(string userId, Document doc)
    {
        if (!IDFManager.userService.ValidateUserPermission(userId, "Document", "Create"))
        {
            return ServiceResult<Document>.Failure("Insufficient permissions");
        }

        // Create document
        return ServiceResult<Document>.Success(document);
    }

    public ServiceResult<Document> UpdateDocument(string userId, Document doc)
    {
        // Check if user is document owner OR has Update permission
        if (doc.OwnerId != userId &&
            !IDFManager.userService.ValidateUserPermission(userId, "Document", "Update"))
        {
            return ServiceResult<Document>.Failure("Insufficient permissions");
        }

        // Update document
        return ServiceResult<Document>.Success(document);
    }
}
```

### Pattern 3: Blazor with Dynamic UI

```razor
@page "/dashboard"
@layout AuthLayout

<h1>Dashboard</h1>

<div class="menu">
    <a href="/orders">Orders</a>

    @if (HasPermission("Product", "Read"))
    {
        <a href="/products">Products</a>
    }

    @if (HasPermission("Report", "View"))
    {
        <a href="/reports">Reports</a>
    }

    @if (IsAdmin())
    {
        <a href="/admin">Admin Panel</a>
    }
</div>

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }

    private bool HasPermission(string entity, string permissionType)
    {
        return IDFManager.userService.ValidateUserPermission(
            authLayoutModel.User.Id,
            entity,
            permissionType
        );
    }

    private bool IsAdmin()
    {
        return IDFManager.userService.ValidateUserRole(
            authLayoutModel.User.Id,
            "Administrators"
        );
    }
}
```

### Pattern 4: Hybrid Authorization

```csharp
public class ReportService
{
    public ServiceResult<Report> DeleteReport(string userId, string reportId)
    {
        // Allow if user is admin OR has specific permission
        bool canDelete = IDFManager.userService.ValidateUserRoleOrPermission(
            userId,
            "Administrators",  // Role
            "Report",          // Entity
            "Delete"           // Permission Type
        );

        if (!canDelete)
        {
            return ServiceResult.Failure("You cannot delete this report");
        }

        // Proceed with deletion
        return ServiceResult.Success();
    }
}
```

## Security Considerations

### 1. Always Validate on Server

```csharp
// WRONG: Client-side only
@if (canDelete)
{
    <button @onclick="DeleteItem">Delete</button>
}

// RIGHT: Client and server validation
@if (canDelete)  // Hide UI
{
    <button @onclick="DeleteItem">Delete</button>
}

@code {
    private async Task DeleteItem()
    {
        // Server validates again
        var result = await api.DeleteItem(itemId, token);
    }
}
```

### 2. Use HTTPS in Production

```csharp
// Enforce HTTPS
app.UseHttpsRedirection();
app.UseHsts();
```

### 3. Secure Token Storage

```csharp
// Good: Encrypted storage
await protectedLocalStorage.SetAsync("token", token);

// Bad: Plain text
localStorage.setItem("token", token);
```

### 4. Validate Tokens on Every Request

```csharp
[IDFAuthUser]  // Validates token automatically
public IActionResult GetData()
{
    // Token already validated by filter
}
```

### 5. Use Appropriate Token Lifetime

```csharp
// Sensitive operations: Short-lived
DefaultTokenLifeTime = 30  // 30 minutes

// Standard apps: Medium
DefaultTokenLifeTime = 60  // 1 hour

// Background services: Long-lived
DefaultTokenLifeTime = 1440  // 24 hours
```

### 6. Implement Rate Limiting

Prevent brute-force attacks on login endpoint.

### 7. Log Authentication Failures

Monitor for suspicious activity.

### 8. Use Permissions for Granular Control

```csharp
// Better: Granular permissions
bool canExport = ValidateUserPermission(userId, "Report", "Export");

// Less secure: Broad roles
bool isAdmin = ValidateUserRole(userId, "Administrators");
```

---

[← Back to README](README.md) | [API Reference →](API-Reference.md)
