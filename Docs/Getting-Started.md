# Getting Started with CMouss.IdentityFramework

This guide will help you quickly set up and start using the CMouss.IdentityFramework in your .NET application.

## Table of Contents

- [Installation](#installation)
- [Quick Start - Console/Desktop Application](#quick-start---consoledesktop-application)
- [Quick Start - ASP.NET Core Web API](#quick-start---aspnet-core-web-api)
- [Quick Start - Blazor Server Application](#quick-start---blazor-server-application)
- [Next Steps](#next-steps)

## Installation

### Prerequisites

- .NET 8.0 SDK or higher
- Visual Studio 2022 / VS Code / JetBrains Rider
- SQL Server 2016+ or SQLite 3.0+

### Install NuGet Packages

Choose the packages based on your project type:

#### For Console/Library Projects

```bash
dotnet add package CMouss.IdentityFramework
```

#### For Web API Projects

```bash
dotnet add package CMouss.IdentityFramework
dotnet add package CMouss.IdentityFramework.APIServer
```

#### For Blazor Server Projects

```bash
dotnet add package CMouss.IdentityFramework
dotnet add package CMouss.IdentityFramework.BlazorUI
```

## Quick Start - Console/Desktop Application

### Step 1: Configure IDFManager

Create a configuration in your `Program.cs` or startup code:

```csharp
using CMouss.IdentityFramework;
using CMouss.IdentityFramework.Models;

// Configure the Identity Framework
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",

    // Administrator account
    AdministratorUserName = "admin",
    AdministratorPassword = "Admin@123",
    AdministratorRoleName = "Administrators",

    // Token settings
    TokenEncryptionKey = "MySecretKey12345", // Use a secure key in production
    DefaultTokenLifeTime = 60, // Minutes
    TokenValidationMode = TokenValidationMode.DecryptOnly,

    // User settings
    IsActiveByDefault = true,
    IsLockedByDefault = false,
    AllowUserMultipleSessions = false
});
```

### Step 2: Initialize Database

```csharp
// Create and initialize the database
using var db = new IDFDBContext();
db.Database.EnsureCreated();
db.InsertMasterData(); // Creates admin user and role
```

### Step 3: Use the Framework

```csharp
// Login a user
var authResult = IDFManager.authService.AuthUserLogin(
    "admin",
    "Admin@123",
    "127.0.0.1"
);

if (authResult.Status == SecurityValidationResult.Ok)
{
    Console.WriteLine($"Login successful! Token: {authResult.UserToken.Token}");
    Console.WriteLine($"User: {authResult.User.FullName}");
}
else
{
    Console.WriteLine($"Login failed: {authResult.Status}");
}

// Create a new user
var newUser = IDFManager.userService.Create(new User
{
    UserName = "john.doe",
    Password = "John@123",
    FullName = "John Doe",
    Email = "john@example.com"
});

if (newUser.IsSuccessful)
{
    Console.WriteLine($"User created with ID: {newUser.Result.Id}");
}

// Grant a role to the user
IDFManager.userService.GrantRole(newUser.Result.Id, "Administrators");

// Check permissions
bool hasPermission = IDFManager.userService.ValidateUserPermission(
    newUser.Result.Id,
    "User",
    "Create"
);

Console.WriteLine($"User has 'User:Create' permission: {hasPermission}");
```

## Quick Start - ASP.NET Core Web API

### Step 1: Install Packages

```bash
dotnet add package CMouss.IdentityFramework
dotnet add package CMouss.IdentityFramework.APIServer
```

### Step 2: Configure in Startup.cs or Program.cs

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
    TokenEncryptionKey = "MySecretKey12345",
    DefaultTokenLifeTime = 60
});

// Add controllers
builder.Services.AddControllers();
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

### Step 3: Use Built-in API Controllers

The framework automatically provides these endpoints:

```
POST /api/Identity/Auth/UserLogin
POST /api/Identity/Auth/UserToken
POST /api/Identity/User/Register
GET  /api/Identity/User/Search
GET  /api/Identity/User/Details/{id}
POST /api/Identity/User/Create
PUT  /api/Identity/User/Update
DELETE /api/Identity/User/Delete/{id}
POST /api/Identity/User/GrantRole
POST /api/Identity/User/RevokeRole
```

### Step 4: Create Custom Protected Controllers

```csharp
using Microsoft.AspNetCore.Mvc;
using CMouss.IdentityFramework.API.Serving.ActionFilters;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    // Requires authentication
    [HttpGet]
    [IDFAuthUser]
    public IActionResult GetOrders()
    {
        return Ok(new { message = "Your orders" });
    }

    // Requires specific role
    [HttpPost]
    [IDFAuthUserWithRoles("Administrators")]
    public IActionResult CreateOrder([FromBody] object order)
    {
        return Ok(new { message = "Order created" });
    }

    // Requires specific permission
    [HttpPut("{id}")]
    [IDFAuthUserWithPermissions("Order:Update")]
    public IActionResult UpdateOrder(string id, [FromBody] object order)
    {
        return Ok(new { message = $"Order {id} updated" });
    }

    // Requires role OR permission
    [HttpDelete("{id}")]
    [IDFAuthUserWithRolesOrPermissions("Administrators", "Order:Delete")]
    public IActionResult DeleteOrder(string id)
    {
        return Ok(new { message = $"Order {id} deleted" });
    }
}
```

### Step 5: Test the API

#### Login Request

```http
POST /api/Identity/Auth/UserLogin
Content-Type: application/json

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
  "userToken": {
    "token": "encrypted-token-string",
    "expireDate": "2024-01-15T10:30:00"
  },
  "user": {
    "id": "user-id",
    "userName": "admin",
    "fullName": "Administrator"
  }
}
```

#### Call Protected Endpoint

```http
GET /api/orders
userToken: encrypted-token-string
```

## Quick Start - Blazor Server Application

### Step 1: Install Packages

```bash
dotnet add package CMouss.IdentityFramework
dotnet add package CMouss.IdentityFramework.BlazorUI
```

### Step 2: Configure in Program.cs

```csharp
using CMouss.IdentityFramework;
using CMouss.IdentityFramework.BlazorUI;
using CMouss.IdentityFramework.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configure Identity Framework
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",
    AdministratorUserName = "admin",
    AdministratorPassword = "Admin@123",
    TokenEncryptionKey = "MySecretKey12345"
});

// Configure Blazor UI
IDFBlazorUIConfig.HomeURL = "/";
IDFBlazorUIConfig.AuthHomeURL = "/dashboard";
IDFBlazorUIConfig.LoginRedirectURL = "/login";
IDFBlazorUIConfig.AfterLogoutRedirectURL = "/login";

// Customize form labels (optional)
IDFBlazorUIConfig.FormLabels.UserName = "Email or Username";
IDFBlazorUIConfig.FormLabels.LoginButton = "Sign In";

var app = builder.Build();

// Initialize database
using (var db = new IDFDBContext())
{
    db.Database.EnsureCreated();
    db.InsertMasterData();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
```

### Step 3: Create Login Page

Create `Pages/Login.razor`:

```razor
@page "/login"
@layout AuthEmptyLayout

<div class="login-container">
    <h2>Login</h2>
    <LoginPart />
</div>
```

### Step 4: Create Protected Pages

Create `Pages/Dashboard.razor`:

```razor
@page "/dashboard"
@layout AuthLayout

<h1>Welcome, @authLayoutModel.User.FullName!</h1>

<p>Email: @authLayoutModel.User.Email</p>
<p>User ID: @authLayoutModel.User.Id</p>

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }
}
```

### Step 5: Create Main Layout with Authentication

Update `Shared/MainLayout.razor`:

```razor
@inherits LayoutComponentBase
@layout AuthLayout

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row px-4">
            <a href="/logout">Logout</a>
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }
}
```

### Step 6: Run and Test

1. Run your application
2. Navigate to `/login`
3. Login with credentials: `admin` / `Admin@123`
4. You'll be redirected to `/dashboard`

## Common Configuration Options

### SQLite Database

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;"
});
```

### SQL Server Database

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.MSSQL,
    DBConnectionString = "Server=localhost;Database=IdentityDB;Trusted_Connection=True;"
});
```

### LDAP/Active Directory Authentication

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",
    AuthenticationBackend = AuthenticationBackend.LDAP,
    LDAPServerAddress = "ldap://dc.company.com",
    LDAPBindDN = "CN=Admin,DC=company,DC=com",
    LDAPBindPassword = "password",
    LDAPSearchBase = "DC=company,DC=com",
    LDAPUserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))"
});
```

### Token Validation Modes

#### Fast Mode (DecryptOnly)
Best for high-performance scenarios:

```csharp
TokenValidationMode = TokenValidationMode.DecryptOnly
```

#### Secure Mode (DecryptAndValidate)
Validates against database on each request:

```csharp
TokenValidationMode = TokenValidationMode.DecryptAndValidate
```

## Next Steps

Now that you have the framework set up, explore these guides:

- [Configuration Guide](Configuration-Guide.md) - Detailed configuration options
- [Authentication & Authorization](Authentication-Authorization.md) - Security implementation
- [CMouss.IdentityFramework](CMouss.IdentityFramework.md) - Core framework documentation
- [CMouss.IdentityFramework.APIServer](CMouss.IdentityFramework.APIServer.md) - API documentation
- [CMouss.IdentityFramework.BlazorUI](CMouss.IdentityFramework.BlazorUI.md) - Blazor UI documentation
- [API Reference](API-Reference.md) - Complete API reference

## Troubleshooting

### Database Creation Issues

If the database doesn't create automatically:

```csharp
using var db = new IDFDBContext();
db.Database.EnsureDeleted(); // Delete existing database
db.Database.EnsureCreated();  // Create new database
db.InsertMasterData();        // Insert master data
```

### Token Validation Failures

Ensure:
1. The `TokenEncryptionKey` is the same across application restarts
2. The token hasn't expired (check `DefaultTokenLifeTime`)
3. In API calls, the token is sent in the `userToken` header

### LDAP Connection Issues

Check:
1. LDAP server address is accessible
2. Bind credentials are correct
3. Search base DN is valid
4. User search filter matches your AD schema

## Sample Projects

Check the included sample projects:

- `BlazorUIDemo` - Complete Blazor Server application
- API examples in the APIServer package

---

[← Back to README](README.md) | [Configuration Guide →](Configuration-Guide.md)
