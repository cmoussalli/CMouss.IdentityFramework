# Quick Start Guide - CMouss.IdentityFramework.BlazorUI

## Installation & Setup

### 1. Add the Package Reference
Add a reference to the `CMouss.IdentityFramework.BlazorUI` package in your Blazor Server application.

### 2. Register Services (One Line!)
In your `Program.cs`, add the following using directive and register the services:

```csharp
using CMouss.IdentityFramework.BlazorUI;

public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Add Blazor components
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // Register Identity Framework BlazorUI services ✨
    builder.Services.AddIdentityFrameworkBlazorUI();

    var app = builder.Build();

    // ... rest of your configuration
}
```

That's it! The `AddIdentityFrameworkBlazorUI()` extension method automatically registers:
- `CookieAuthService` - Cookie-based authentication
- Any future services added to the library

### 3. Configure Identity Framework
Configure the core Identity Framework as usual:

```csharp
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=database.db;",
    TokenEncryptionKey = "YourSecureKey123",
    AdministratorUserName = "admin",
    AdministratorPassword = "admin",
    // ... other settings
});
```

### 4. Configure BlazorUI URLs (Optional)
Customize the authentication flow URLs:

```csharp
IDFBlazorUIConfig.AuthHomeURL = "/dashboard";
IDFBlazorUIConfig.LoginRedirectURL = "/login";
IDFBlazorUIConfig.AfterLogoutRedirectURL = "/";
```

## Usage in Components

### Protected Pages
Use `AuthLayout` to protect your pages:

```razor
@page "/dashboard"
@layout MainLayoutAuth  <!-- Your layout that wraps AuthLayout -->

<h3>Protected Dashboard</h3>
<p>Welcome, @authLayoutModel.User.FullName!</p>

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }
}
```

### Login Page
Use the built-in `LoginPart` component:

```razor
@page "/login"
@layout AuthEmptyLayout

<div class="container">
    <LoginPart />
</div>
```

### Signup Page
Use the built-in `SignUpPart` component:

```razor
@page "/signup"
@layout AuthEmptyLayout

<div class="container">
    <SignUpPart />
</div>
```

### Logout
Simply navigate to `/logout` or create a link:

```razor
<a href="/logout">Logout</a>
```

## Key Features

✅ **Cookie-Based Authentication** - Secure token storage in browser cookies
✅ **CSRF Protection** - SameSite=Strict cookie policy
✅ **Input Validation** - Built-in validation for login and signup
✅ **Email Validation** - Proper email format checking
✅ **Password Requirements** - Minimum 6 characters
✅ **Error Handling** - User-friendly error messages
✅ **Token Encryption** - Tokens encrypted by framework's TokenEncryptionKey
✅ **Auto Expiration** - 30-day cookie expiration by default

## Advanced Usage

### Custom Cookie Expiration
You can customize the cookie expiration when setting tokens:

```csharp
@inject CookieAuthService CookieAuth

// Set token with custom 7-day expiration
await CookieAuth.SetTokenAsync(token, TimeSpan.FromDays(7));
```

### Check Authentication Status
```csharp
@inject CookieAuthService CookieAuth

if (await CookieAuth.HasTokenAsync())
{
    // User is authenticated
}
```

### Manual Token Operations
```csharp
@inject CookieAuthService CookieAuth

// Get token
var token = await CookieAuth.GetTokenAsync();

// Delete token
await CookieAuth.DeleteTokenAsync();
```

## Migration from Local Storage

If you're migrating from the previous `ProtectedLocalStorage` implementation:

1. **Update Program.cs**: Replace `builder.Services.AddScoped<CookieAuthService>()` with `builder.Services.AddIdentityFrameworkBlazorUI()`
2. **Rebuild your application**
3. **Test authentication flow**

See [MIGRATION_GUIDE.md](MIGRATION_GUIDE.md) for detailed migration instructions.

## Troubleshooting

### Service Not Registered
**Error**: "Unable to resolve service for type 'CookieAuthService'"
**Solution**: Ensure you've added `builder.Services.AddIdentityFrameworkBlazorUI()` in `Program.cs`

### Authentication Not Persisting
**Solution**: Check browser console for JavaScript errors. Ensure cookies are enabled in your browser.

### Headers Already Started Error
**Solution**: This has been resolved in the current version. If you still see this, ensure you're using the latest version of the library.

## Support & Documentation

- **Full Migration Guide**: [MIGRATION_GUIDE.md](MIGRATION_GUIDE.md)
- **Detailed Changes**: [COOKIE_AUTH_UPDATE.md](COOKIE_AUTH_UPDATE.md)
- **Framework Documentation**: See main CMouss.IdentityFramework documentation

## Minimum Requirements

- .NET 9.0 or later
- Blazor Server with Interactive rendering
- Modern browser with cookie support
