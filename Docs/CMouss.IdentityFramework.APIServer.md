# CMouss.IdentityFramework.APIServer

ASP.NET Core API controllers and action filters for building REST APIs with authentication and authorization.

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Setup](#setup)
- [Built-in Controllers](#built-in-controllers)
- [Action Filters (Authorization Attributes)](#action-filters-authorization-attributes)
- [API Routes](#api-routes)
- [Creating Protected Controllers](#creating-protected-controllers)
- [Request/Response Examples](#requestresponse-examples)
- [Error Handling](#error-handling)

## Overview

CMouss.IdentityFramework.APIServer provides:

- **Pre-built API Controllers**: Ready-to-use REST endpoints for authentication and user management
- **Authorization Action Filters**: Attributes to protect endpoints with role and permission checks
- **Standardized API Routes**: Consistent URL patterns
- **Token-based Authentication**: Secure API access using encrypted tokens

## Installation

```bash
dotnet add package CMouss.IdentityFramework
dotnet add package CMouss.IdentityFramework.APIServer
```

### Dependencies

- ASP.NET Core 8.0+
- CMouss.IdentityFramework
- Microsoft.AspNetCore.Mvc.ViewFeatures

## Setup

### Configure in Program.cs (Minimal API)

```csharp
using CMouss.IdentityFramework;
using CMouss.IdentityFramework.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure Identity Framework
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",
    AdministratorUserName = "admin",
    AdministratorPassword = "Admin@123",
    TokenEncryptionKey = "YourSecretKey123",
    DefaultTokenLifeTime = 60
});

// Add services
builder.Services.AddControllers()
    .AddApplicationPart(typeof(CMouss.IdentityFramework.API.Serving.Controllers.IDFAuthController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database
using (var db = new IDFDBContext())
{
    db.Database.EnsureCreated();
    db.InsertMasterData();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Configure in Startup.cs (Traditional)

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Configure Identity Framework
    IDFManager.Configure(new IDFManagerConfig
    {
        DatabaseType = DatabaseType.SQLite,
        DBConnectionString = "Data Source=identity.db;",
        AdministratorUserName = "admin",
        AdministratorPassword = "Admin@123",
        TokenEncryptionKey = "YourSecretKey123"
    });

    services.AddControllers()
        .AddApplicationPart(typeof(CMouss.IdentityFramework.API.Serving.Controllers.IDFAuthController).Assembly);
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // Initialize database
    using (var db = new IDFDBContext())
    {
        db.Database.EnsureCreated();
        db.InsertMasterData();
    }

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRouting();
    app.UseAuthorization();
    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
}
```

## Built-in Controllers

### IDFAuthController

Handles authentication operations.

#### Endpoints

**POST /api/Identity/Auth/UserLogin**

Login with username and password.

Request:
```json
{
  "userName": "admin",
  "password": "Admin@123",
  "ipAddress": "127.0.0.1"
}
```

Response:
```json
{
  "status": "Ok",
  "authenticationMode": "User",
  "userToken": {
    "token": "encrypted-token-string",
    "expireDate": "2024-01-15T10:30:00",
    "userId": "user-id",
    "ipAddress": "127.0.0.1"
  },
  "user": {
    "id": "user-id",
    "userName": "admin",
    "fullName": "Administrator",
    "email": "admin@example.com",
    "isActive": true
  }
}
```

**POST /api/Identity/Auth/UserToken**

Validate a user token.

Request:
```json
{
  "token": "encrypted-token-string",
  "ipAddress": "127.0.0.1"
}
```

Response: Same as UserLogin

**POST /api/Identity/Auth/AppLogin**

Login for application accounts.

**POST /api/Identity/Auth/AppAccess**

Validate app access token.

### IDFUserController

Complete user management API.

All endpoints (except Register) require authentication via action filters.

#### Endpoints

**POST /api/Identity/User/Register**

User self-registration (no authentication required).

Request:
```json
{
  "userName": "john.doe",
  "password": "Password123!",
  "fullName": "John Doe",
  "email": "john@example.com"
}
```

**GET /api/Identity/User/Search**

Search users with pagination.

Headers: `userToken: encrypted-token-string`

Query Parameters:
- `searchTerm`: Search text (optional)
- `pageNumber`: Page number (default: 1)
- `pageSize`: Items per page (default: 10)
- `includeDeleted`: Include deleted users (default: false)
- `includeLocked`: Include locked users (default: false)

**GET /api/Identity/User/Details/{id}**

Get user details by ID.

Headers: `userToken: encrypted-token-string`

**POST /api/Identity/User/Create**

Create a new user (admin operation).

Headers: `userToken: encrypted-token-string`

Request:
```json
{
  "userName": "jane.doe",
  "password": "Password123!",
  "fullName": "Jane Doe",
  "email": "jane@example.com",
  "isActive": true,
  "isLocked": false
}
```

**PUT /api/Identity/User/Update**

Update user information.

Headers: `userToken: encrypted-token-string`

**DELETE /api/Identity/User/Delete/{id}**

Soft delete a user.

Headers: `userToken: encrypted-token-string`

**POST /api/Identity/User/ChangePassword**

Admin changes user password.

Headers: `userToken: encrypted-token-string`

Request:
```json
{
  "userId": "user-id",
  "newPassword": "NewPassword123!"
}
```

**POST /api/Identity/User/ChangeMyPassword**

User changes their own password.

Headers: `userToken: encrypted-token-string`

Request:
```json
{
  "userId": "user-id",
  "oldPassword": "OldPassword123!",
  "newPassword": "NewPassword123!"
}
```

**POST /api/Identity/User/Lock/{id}**

Lock a user account.

**POST /api/Identity/User/Unlock/{id}**

Unlock a user account.

**GET /api/Identity/User/GetRoles/{userId}**

Get user's roles.

**POST /api/Identity/User/GrantRole**

Grant a role to user.

Request:
```json
{
  "userId": "user-id",
  "roleId": "role-id"
}
```

**POST /api/Identity/User/RevokeRole**

Revoke a role from user.

**POST /api/Identity/User/ValidateUserRole**

Check if user has a specific role.

Request:
```json
{
  "userId": "user-id",
  "roleId": "Administrators"
}
```

Response:
```json
{
  "hasRole": true
}
```

**POST /api/Identity/User/ValidateUserAnyRole**

Check if user has any of the specified roles.

**POST /api/Identity/User/ValidateTokenRole**

Validate token and check for role.

### IDFAppController

Application management API.

#### Endpoints

**GET /api/Identity/App/Search**

Search applications.

**GET /api/Identity/App/Details/{id}**

Get app details.

**GET /api/Identity/App/GetAll**

Get all applications.

**POST /api/Identity/App/Create**

Create new application.

**PUT /api/Identity/App/Update**

Update application.

**DELETE /api/Identity/App/Delete/{id}**

Delete application.

**GET /api/Identity/App/GetUserApps/{userId}**

Get all apps for a specific user.

## Action Filters (Authorization Attributes)

Authorization attributes to protect your API endpoints.

### IDFAuthUser

Basic token authentication. Validates that a valid token is provided.

```csharp
[HttpGet]
[IDFAuthUser]
public IActionResult GetProtectedData()
{
    // Only authenticated users can access
    return Ok(new { data = "protected" });
}
```

### IDFAuthUserWithRoles

Requires user to have one or more specific roles.

```csharp
[HttpPost]
[IDFAuthUserWithRoles("Administrators")]
public IActionResult AdminOnlyAction()
{
    // Only users with "Administrators" role
    return Ok();
}

[HttpPost]
[IDFAuthUserWithRoles("Administrators,PowerUsers")]
public IActionResult MultiRoleAction()
{
    // Users with "Administrators" OR "PowerUsers" role
    return Ok();
}
```

**Format**: Comma-separated role IDs

### IDFAuthUserWithPermissions

Requires user to have specific permissions (Entity:PermissionType format).

```csharp
[HttpPost]
[IDFAuthUserWithPermissions("Order:Create")]
public IActionResult CreateOrder()
{
    // Only users with "Order:Create" permission
    return Ok();
}

[HttpPut]
[IDFAuthUserWithPermissions("Order:Update,Order:Create")]
public IActionResult UpdateOrder()
{
    // Users must have BOTH "Order:Update" AND "Order:Create"
    return Ok();
}
```

**Format**: Comma-separated Entity:PermissionType pairs

### IDFAuthUserWithRolesOrPermissions

Requires user to have either specified roles OR permissions (any match).

```csharp
[HttpDelete]
[IDFAuthUserWithRolesOrPermissions("Administrators", "Order:Delete")]
public IActionResult DeleteOrder()
{
    // Users with "Administrators" role OR "Order:Delete" permission
    return Ok();
}
```

**Parameters**:
1. Roles (comma-separated)
2. Permissions (comma-separated Entity:PermissionType pairs)

### How Filters Work

1. Extract `userToken` from request header
2. Validate token using AuthService
3. Check roles/permissions if specified
4. Return **403 Forbidden** if authorization fails
5. Allow request to proceed if authorized

## API Routes

All routes are under the base path: `/api/Identity`

### APIRoutes Class

```csharp
public static class APIRoutes
{
    public const string BaseRoute = "api/Identity";

    public static class Auth
    {
        public const string UserLogin = BaseRoute + "/Auth/UserLogin";
        public const string UserToken = BaseRoute + "/Auth/UserToken";
        public const string AppLogin = BaseRoute + "/Auth/AppLogin";
        public const string AppAccess = BaseRoute + "/Auth/AppAccess";
    }

    public static class User
    {
        public const string Register = BaseRoute + "/User/Register";
        public const string Search = BaseRoute + "/User/Search";
        public const string Details = BaseRoute + "/User/Details";
        public const string Create = BaseRoute + "/User/Create";
        public const string Update = BaseRoute + "/User/Update";
        public const string Delete = BaseRoute + "/User/Delete";
        public const string ChangePassword = BaseRoute + "/User/ChangePassword";
        public const string ChangeMyPassword = BaseRoute + "/User/ChangeMyPassword";
        public const string Lock = BaseRoute + "/User/Lock";
        public const string Unlock = BaseRoute + "/User/Unlock";
        public const string GetRoles = BaseRoute + "/User/GetRoles";
        public const string GrantRole = BaseRoute + "/User/GrantRole";
        public const string RevokeRole = BaseRoute + "/User/RevokeRole";
    }

    public static class App
    {
        public const string Search = BaseRoute + "/App/Search";
        public const string Details = BaseRoute + "/App/Details";
        public const string GetAll = BaseRoute + "/App/GetAll";
        public const string Create = BaseRoute + "/App/Create";
        public const string Update = BaseRoute + "/App/Update";
        public const string Delete = BaseRoute + "/App/Delete";
    }
}
```

## Creating Protected Controllers

### Example 1: Basic Protected Controller

```csharp
using Microsoft.AspNetCore.Mvc;
using CMouss.IdentityFramework.API.Serving.ActionFilters;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [IDFAuthUser] // Requires authentication only
    public IActionResult GetProducts()
    {
        // Any authenticated user can access
        return Ok(new[] { "Product1", "Product2" });
    }

    [HttpGet("{id}")]
    [IDFAuthUser]
    public IActionResult GetProduct(string id)
    {
        return Ok(new { id, name = "Product Name" });
    }
}
```

### Example 2: Role-Based Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    [IDFAuthUser]
    public IActionResult GetOrders()
    {
        // Any authenticated user can view orders
        return Ok(new[] { "Order1", "Order2" });
    }

    [HttpPost]
    [IDFAuthUserWithRoles("OrderManager,Administrators")]
    public IActionResult CreateOrder([FromBody] OrderDto order)
    {
        // Only OrderManager or Administrators can create
        return Ok(new { message = "Order created" });
    }

    [HttpDelete("{id}")]
    [IDFAuthUserWithRoles("Administrators")]
    public IActionResult DeleteOrder(string id)
    {
        // Only Administrators can delete
        return Ok(new { message = $"Order {id} deleted" });
    }
}
```

### Example 3: Permission-Based Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [HttpGet("sales")]
    [IDFAuthUserWithPermissions("Report:View")]
    public IActionResult GetSalesReport()
    {
        // Requires "Report:View" permission
        return Ok(new { report = "Sales data" });
    }

    [HttpPost("generate")]
    [IDFAuthUserWithPermissions("Report:Generate")]
    public IActionResult GenerateReport([FromBody] ReportRequest request)
    {
        // Requires "Report:Generate" permission
        return Ok(new { message = "Report generated" });
    }

    [HttpDelete("{id}")]
    [IDFAuthUserWithPermissions("Report:Delete")]
    public IActionResult DeleteReport(string id)
    {
        // Requires "Report:Delete" permission
        return Ok(new { message = "Report deleted" });
    }
}
```

### Example 4: Mixed Role and Permission Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    [IDFAuthUserWithRolesOrPermissions("Administrators", "User:View")]
    public IActionResult GetUsers()
    {
        // Administrators OR users with "User:View" permission
        return Ok(new[] { "User1", "User2" });
    }

    [HttpPost]
    [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Create")]
    public IActionResult CreateUser([FromBody] UserDto user)
    {
        // Administrators OR users with "User:Create" permission
        return Ok(new { message = "User created" });
    }

    [HttpDelete("{id}")]
    [IDFAuthUserWithRoles("Administrators")]
    public IActionResult DeleteUser(string id)
    {
        // Only Administrators (no permission alternative)
        return Ok(new { message = "User deleted" });
    }
}
```

### Example 5: Accessing User Information in Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    [HttpGet("me")]
    [IDFAuthUser]
    public IActionResult GetMyProfile()
    {
        // Get user token from header
        string token = Request.Headers["userToken"].FirstOrDefault();

        // Validate and get user info
        var authResult = IDFManager.authService.AuthUserToken(
            token,
            IDFManager.config.TokenValidationMode,
            HttpContext.Connection.RemoteIpAddress?.ToString()
        );

        if (authResult.Status == SecurityValidationResult.Ok)
        {
            var user = authResult.User;
            return Ok(new
            {
                user.Id,
                user.UserName,
                user.FullName,
                user.Email
            });
        }

        return Unauthorized();
    }

    [HttpPut("me")]
    [IDFAuthUser]
    public IActionResult UpdateMyProfile([FromBody] ProfileUpdateDto dto)
    {
        string token = Request.Headers["userToken"].FirstOrDefault();
        var authResult = IDFManager.authService.AuthUserToken(
            token,
            IDFManager.config.TokenValidationMode,
            HttpContext.Connection.RemoteIpAddress?.ToString()
        );

        if (authResult.Status == SecurityValidationResult.Ok)
        {
            var user = authResult.User;
            user.FullName = dto.FullName;
            user.Email = dto.Email;

            var updateResult = IDFManager.userService.Update(user);
            return Ok(updateResult);
        }

        return Unauthorized();
    }
}
```

## Request/Response Examples

### Authentication Flow

#### 1. Login

```http
POST /api/Identity/Auth/UserLogin HTTP/1.1
Content-Type: application/json

{
  "userName": "john.doe",
  "password": "Password123!",
  "ipAddress": "192.168.1.100"
}
```

Response:
```json
{
  "status": "Ok",
  "authenticationMode": "User",
  "userToken": {
    "token": "A8F3C2E1B4D7F9A2...",
    "expireDate": "2024-01-15T12:30:00Z",
    "userId": "usr_123456",
    "ipAddress": "192.168.1.100"
  },
  "user": {
    "id": "usr_123456",
    "userName": "john.doe",
    "fullName": "John Doe",
    "email": "john@example.com",
    "isActive": true,
    "isLocked": false
  }
}
```

#### 2. Call Protected Endpoint

```http
GET /api/orders HTTP/1.1
userToken: A8F3C2E1B4D7F9A2...
```

Response (Success):
```json
{
  "orders": [
    { "id": "ord_001", "total": 150.00 },
    { "id": "ord_002", "total": 200.00 }
  ]
}
```

Response (Unauthorized - 403):
```json
{
  "error": "Unauthorized",
  "message": "You do not have permission to access this resource"
}
```

### User Management Flow

#### Create User

```http
POST /api/Identity/User/Create HTTP/1.1
Content-Type: application/json
userToken: A8F3C2E1B4D7F9A2...

{
  "userName": "jane.smith",
  "password": "SecurePass123!",
  "fullName": "Jane Smith",
  "email": "jane@example.com",
  "isActive": true
}
```

#### Grant Role

```http
POST /api/Identity/User/GrantRole HTTP/1.1
Content-Type: application/json
userToken: A8F3C2E1B4D7F9A2...

{
  "userId": "usr_789",
  "roleId": "OrderManager"
}
```

#### Search Users

```http
GET /api/Identity/User/Search?searchTerm=john&pageNumber=1&pageSize=10 HTTP/1.1
userToken: A8F3C2E1B4D7F9A2...
```

## Error Handling

### Standard Error Responses

#### 401 Unauthorized
Missing or invalid token.

```json
{
  "error": "Unauthorized",
  "message": "Valid authentication token required"
}
```

#### 403 Forbidden
Valid token but insufficient permissions.

```json
{
  "error": "Forbidden",
  "message": "You do not have the required role or permission"
}
```

#### 400 Bad Request
Invalid request data.

```json
{
  "error": "Bad Request",
  "message": "Invalid user data provided"
}
```

#### 500 Internal Server Error
Server-side error.

```json
{
  "error": "Internal Server Error",
  "message": "An unexpected error occurred"
}
```

### Custom Error Handling

```csharp
[ApiController]
[Route("api/[controller]")]
public class CustomController : ControllerBase
{
    [HttpPost]
    [IDFAuthUser]
    public IActionResult CreateItem([FromBody] ItemDto item)
    {
        try
        {
            // Your logic here
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Internal Server Error",
                message = ex.Message
            });
        }
    }
}
```

## Best Practices

1. **Always use HTTPS** in production to protect tokens in transit
2. **Store TokenEncryptionKey securely** (environment variables, Azure Key Vault, etc.)
3. **Set appropriate token lifetime** based on your security requirements
4. **Use permission-based authorization** for granular control
5. **Log authentication failures** for security monitoring
6. **Implement rate limiting** to prevent brute-force attacks
7. **Validate input data** in your controllers
8. **Return appropriate HTTP status codes**
9. **Don't expose sensitive information** in error messages

## Testing with Swagger

The framework works seamlessly with Swagger/OpenAPI.

Add Swagger configuration:

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Identity API",
        Version = "v1"
    });

    // Add security definition for userToken header
    c.AddSecurityDefinition("UserToken", new OpenApiSecurityScheme
    {
        Description = "User authentication token",
        Name = "userToken",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "UserToken"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "UserToken"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

---

[← Back to README](README.md) | [BlazorUI Documentation →](CMouss.IdentityFramework.BlazorUI.md)
