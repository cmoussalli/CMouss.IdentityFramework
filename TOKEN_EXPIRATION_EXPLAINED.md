# Token Expiration: How It Works

## Overview

The authentication system uses **TWO independent expiration mechanisms** that work together:

1. **Cookie Expiration** (Browser-side) - 30 days default
2. **Token Expiration** (Framework-side) - Configured via `DefaultTokenLifeTime`

## Detailed Explanation

### 1. Cookie Expiration (CookieAuthService)

**Location**: Browser cookies
**Default**: 30 days
**Configured in**: `CookieAuthService.SetTokenAsync()`

```csharp
private static readonly TimeSpan DefaultCookieExpiration = TimeSpan.FromDays(30);
```

**What it does**:
- The browser automatically deletes the cookie after 30 days
- This is a **hard limit** - even if the token is still valid, it won't be accessible
- This is a **client-side protection** mechanism

### 2. Token Expiration (IDFManager)

**Location**: Encrypted within the token itself
**Default**: Configured in your application
**Configured in**: `IDFManager.Configure()`

```csharp
DefaultTokenLifeTime = new LifeTime(365, 0, 0)  // 365 days
```

**What it does**:
- When a token is created, it includes an expiration date based on `DefaultTokenLifeTime`
- The expiration date is encrypted inside the token
- When `IDFManager.userTokenService.Validate()` is called, it checks if the token has expired
- This is a **server-side security** mechanism

## How They Work Together

### Scenario 1: Cookie Expires First (30 days)
```
Day 1:  User logs in → Cookie set (30 days) + Token created (365 days)
Day 30: Cookie expires → Browser deletes cookie
Day 31: User tries to access → No cookie found → Redirected to login
        (Token is still valid but inaccessible)
```

**Result**: User must login again after 30 days

### Scenario 2: Token Expires First (if cookie expiration > token lifetime)
```
Day 1:   User logs in → Cookie set (365 days) + Token created (30 days)
Day 30:  Token expires (but cookie still exists)
Day 31:  User tries to access → Cookie retrieved → Token validated
         → Token expired → Validation fails → Redirected to login
```

**Result**: User must login again after 30 days

### Scenario 3: Current Configuration (Cookie: 30d, Token: 365d)
```
Day 1:   User logs in → Cookie set (30 days) + Token created (365 days)
Day 30:  Cookie expires → No cookie to retrieve
Day 365: Token would expire, but cookie already gone at day 30
```

**Result**: User must login again after **30 days** (whichever expires first)

## Code Flow

### Login (Token Creation)
```csharp
// In LoginPart.razor
AuthResult authResult = IDFManager.authService.AuthUserLogin(userName, password);

// Creates token with DefaultTokenLifeTime (365 days in your config)
// Token contains: UserId, UserName, Roles, ExpireDate, etc.

// Store in cookie with 30-day expiration
await CookieAuth.SetTokenAsync(authResult.UserToken.Token);
```

### Authentication Check (Token Validation)
```csharp
// In AuthLayout.razor - GetUserInfo()

// Step 1: Get token from cookie
var token = await CookieAuth.GetTokenAsync();
// If cookie expired (30 days), token will be null

if (!string.IsNullOrEmpty(token))
{
    // Step 2: Validate token (checks expiration inside token)
    UserToken userToken = IDFManager.userTokenService.Validate(token, TokenValidationMode.UseDefault);
    // Validates: token format, expiration date (365 days), and optionally database

    if (userToken == null)
    {
        // Token expired or invalid → Redirect to login
        await RedirectToLogin();
    }
    else
    {
        // Token valid → User authenticated
        authLayoutModel.IsAuthenticated = true;
    }
}
```

## Recommendations

### Option 1: Match Cookie and Token Expiration (Recommended)

Update `CookieAuthService` to use the same lifetime as your tokens:

```csharp
// In CookieAuthService.cs
private static readonly TimeSpan DefaultCookieExpiration = TimeSpan.FromDays(365);
```

**Benefits**:
- Token and cookie expire together
- User experience is consistent
- No unnecessary early expiration

### Option 2: Keep Different Expirations (Current)

Keep cookie at 30 days and token at 365 days.

**Benefits**:
- Shorter session duration (more secure)
- Forces re-authentication every 30 days
- Reduces risk if token is compromised

**Trade-offs**:
- Token lifetime (365 days) is never fully utilized
- Users must login every 30 days even though token is valid for 365 days

### Option 3: Make Cookie Expiration Configurable

Add configuration to match `DefaultTokenLifeTime`:

```csharp
// In ServiceCollectionExtensions.cs
public static IServiceCollection AddIdentityFrameworkBlazorUI(
    this IServiceCollection services,
    TimeSpan? cookieExpiration = null)
{
    if (cookieExpiration.HasValue)
    {
        services.Configure<CookieAuthOptions>(options =>
        {
            options.Expiration = cookieExpiration.Value;
        });
    }

    services.AddScoped<CookieAuthService>();
    return services;
}
```

## Current Behavior Summary

With your current configuration:

- **Cookie Expiration**: 30 days (hardcoded in `CookieAuthService`)
- **Token Expiration**: 365 days (configured in `DefaultTokenLifeTime`)
- **Effective Session Duration**: **30 days** (limited by cookie expiration)

**Answer**: Yes, the framework validates token expiry using `DefaultTokenLifeTime`, BUT the cookie expiration (30 days) expires FIRST, so users must login every 30 days regardless of the 365-day token lifetime.

## Validation Modes

Your configuration uses:
```csharp
TokenValidationMode = TokenValidationMode.DecryptOnly
```

This means:
- **DecryptOnly**: Only decrypts token and checks expiration date (fast)
- **DecryptAndValidate**: Also checks database for token existence and validates IP (more secure, slower)

The expiration check happens in **both modes** - it's part of the token claims validation.

## Security Note

Having **different expiration times** can be beneficial:
- Shorter cookie expiration (30 days) = More security
- Longer token expiration (365 days) = Could be used for "remember me" feature (future enhancement)

However, with the current implementation, the longer token lifetime is not utilized because cookies expire first.

## Recommendation for Your Use Case

If you want users to stay logged in for 365 days (as configured in `DefaultTokenLifeTime`), you should update the cookie expiration to match:

**File**: `CMouss.IdentityFramework.BlazorUI/CookieAuthService.cs`

Change:
```csharp
private static readonly TimeSpan DefaultCookieExpiration = TimeSpan.FromDays(30);
```

To:
```csharp
private static readonly TimeSpan DefaultCookieExpiration = TimeSpan.FromDays(365);
```

This ensures the cookie lifetime matches your token lifetime, and users stay authenticated for the full 365 days (until token expiration).
