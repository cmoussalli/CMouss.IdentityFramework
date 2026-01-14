# Migration Guide: Local Storage to Cookie-Based Authentication

This guide will help you migrate from browser local storage to secure cookie-based authentication in your CMouss.IdentityFramework applications.

## Overview

The framework has been updated to use **HTTP-only cookies** instead of browser local storage for storing authentication tokens. This change provides:

- **Enhanced Security**: HTTP-only cookies cannot be accessed by JavaScript, preventing XSS attacks
- **Better CSRF Protection**: SameSite=Strict cookie policy
- **HTTPS Enforcement**: Cookies are only transmitted over secure connections
- **Server-Side Control**: Tokens can be managed from the server side

## What Changed

### Removed Dependencies
- `Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage` is no longer used
- `ProtectedLocalStorage` injections have been removed from all components

### New Service
- **CookieAuthService**: A new service that manages authentication tokens via HTTP-only cookies

### Updated Components
1. **AuthLayout.razor** - Now uses cookies for token retrieval and validation
2. **LoginPart.razor** - Stores tokens in cookies with enhanced validation
3. **Logout.razor** - Properly clears authentication cookies
4. **SignUpPart.razor** - Enhanced validation for user registration

## Migration Steps for Your Application

### Step 1: Register the BlazorUI Services

Update your `Program.cs` file to register the Identity Framework BlazorUI services using the extension method:

```csharp
// Register Identity Framework BlazorUI services
builder.Services.AddIdentityFrameworkBlazorUI();
```

This single line registers all required services including `CookieAuthService`.

**Example:**
```csharp
using CMouss.IdentityFramework.BlazorUI;

public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // Register Identity Framework BlazorUI services
    builder.Services.AddIdentityFrameworkBlazorUI();

    var app = builder.Build();

    // ... rest of your configuration
}
```

**Alternative (Manual Registration):**
If you prefer to register services manually, you can still do:
```csharp
builder.Services.AddScoped<CookieAuthService>();
```

### Step 2: Update Custom Components (If Any)

If you have custom components that previously used `ProtectedLocalStorage`, update them to use `CookieAuthService`:

**Before:**
```razor
@using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage
@inject ProtectedLocalStorage BrowserStorage

@code {
    protected override async Task OnInitializedAsync()
    {
        var result = await BrowserStorage.GetAsync<string>("token");
        if (result.Success)
        {
            // Use token
        }
    }
}
```

**After:**
```razor
@using CMouss.IdentityFramework.BlazorUI
@inject CookieAuthService CookieAuth

@code {
    protected override async Task OnInitializedAsync()
    {
        var token = await CookieAuth.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            // Use token
        }
    }
}
```

### Step 3: Clear Existing Local Storage (Optional)

Since users might have tokens stored in local storage from the previous version, you may want to add a one-time cleanup script or inform users to clear their browser data.

## API Reference: CookieAuthService

### Methods

#### `SetTokenAsync(string token, TimeSpan? expiration = null)`
Stores the authentication token in an HTTP-only cookie.

**Parameters:**
- `token` - The authentication token to store
- `expiration` - Optional custom expiration time (default: 30 days)

**Example:**
```csharp
await CookieAuth.SetTokenAsync(authResult.UserToken.Token);
```

#### `GetTokenAsync()`
Retrieves the authentication token from the cookie.

**Returns:** The authentication token, or `null` if not found

**Example:**
```csharp
var token = await CookieAuth.GetTokenAsync();
if (!string.IsNullOrEmpty(token))
{
    // Token exists
}
```

#### `DeleteTokenAsync()`
Deletes the authentication token cookie.

**Example:**
```csharp
await CookieAuth.DeleteTokenAsync();
```

#### `HasTokenAsync()`
Checks if an authentication token exists.

**Returns:** `true` if a valid token exists, `false` otherwise

**Example:**
```csharp
if (await CookieAuth.HasTokenAsync())
{
    // User is authenticated
}
```

## Security Features

### Cookie Configuration

The `CookieAuthService` uses JavaScript interop to set cookies with security attributes:

- **SameSite=Strict**: Prevents CSRF attacks by not sending cookies with cross-site requests
- **Path=/**: Cookie available throughout the application
- **Max-Age**: 30-day default expiration (configurable)
- **URI Encoding**: Token values are escaped to prevent cookie injection attacks

**Note**: In Blazor Server Interactive mode, cookies are JavaScript-accessible (not HTTP-only) due to component lifecycle limitations. The tokens themselves are still encrypted by the framework's TokenEncryptionKey.

### Benefits

1. **No Local Storage**: Tokens not stored in browser local storage (better than previous approach)
2. **CSRF Protection**: `SameSite=Strict` prevents cookies from being sent with cross-site requests
3. **Secure by Design**: Cookies respect browser security policies
4. **Automatic Expiration**: Cookies expire after 365 days by default (matches typical token lifetime)
5. **Token Encryption**: Tokens are encrypted by IDFManager before storage (double protection)
6. **Synchronized Expiration**: Cookie expiration aligns with token expiration for consistent user experience

## Validation Enhancements

### SignUp Component
The signup component now includes comprehensive validation:
- Email format validation
- Required field validation
- Password minimum length (6 characters)
- Full name and username validation

### Login Component
The login component now includes:
- Required field validation for username and password
- Clear error messages for validation failures
- Proper handling of authentication errors

### Logout Component
The logout component now:
- Uses async/await properly for cookie deletion
- Includes firstRender check to prevent multiple executions
- Provides proper error handling during logout

## Troubleshooting

### Issue: "The name 'CookieAuth' does not exist"
**Solution:** Ensure you've registered the BlazorUI services in your `Program.cs`:
```csharp
builder.Services.AddIdentityFrameworkBlazorUI();
```

Or manually:
```csharp
builder.Services.AddScoped<CookieAuthService>();
```

### Issue: "Headers are read-only, response has already started"
**Solution:** This has been resolved in the updated implementation. The service now uses JavaScript interop exclusively, avoiding HttpContext response manipulation issues.

### Issue: Cookies not persisting across page refreshes
**Solution:** Ensure cookies are being set with proper path and max-age attributes. Check browser console for any JavaScript errors during cookie operations.

### Issue: Users still logged in after clearing cookies
**Solution:** This is likely a caching issue. Ensure proper navigation with force reload:
```csharp
NavManager.NavigateTo(url, forceLoad: true);
```

## Breaking Changes

### For Library Users
- Remove any direct usage of `ProtectedLocalStorage` for authentication tokens
- Update custom components to use `CookieAuthService` instead
- Register the new services in your `Program.cs`

### No Breaking Changes For
- The core IDFManager API remains unchanged
- Token validation logic remains the same
- Database schema and models are unaffected
- Authentication flow and user experience remain consistent

## Version Compatibility

- **Minimum .NET Version**: .NET 9.0
- **Framework Version**: CMouss.IdentityFramework 1.1.4+
- **BlazorUI Version**: CMouss.IdentityFramework.BlazorUI 1.1.4+

## Support

If you encounter any issues during migration, please:
1. Check this guide thoroughly
2. Review the example implementation in the BlazorUIDemo project
3. Report issues on the project repository

## Best Practices

1. **Always use HTTPS** in production to ensure cookie security
2. **Configure appropriate cookie expiration** based on your security requirements
3. **Monitor token usage** and implement proper session management
4. **Test logout functionality** thoroughly to ensure cookies are properly cleared
5. **Implement proper error handling** in authentication flows
