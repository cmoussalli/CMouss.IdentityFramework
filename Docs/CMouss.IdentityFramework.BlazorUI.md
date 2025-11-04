# CMouss.IdentityFramework.BlazorUI

Pre-built Blazor Server components for authentication UI with secure token management.

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Setup](#setup)
- [Configuration](#configuration)
- [Components](#components)
- [Layouts](#layouts)
- [Usage Examples](#usage-examples)
- [Customization](#customization)
- [Best Practices](#best-practices)

## Overview

CMouss.IdentityFramework.BlazorUI provides ready-to-use Blazor Server components for:

- User login and registration
- Secure token storage using ProtectedLocalStorage
- Authentication state management with cascading parameters
- Automatic session validation
- Pre-built authentication layouts
- Logout functionality

## Installation

```bash
dotnet add package CMouss.IdentityFramework
dotnet add package CMouss.IdentityFramework.BlazorUI
```

### Dependencies

- .NET 8.0+
- Blazor Server
- Microsoft.AspNetCore.Components.Web
- CMouss.IdentityFramework

## Setup

### Step 1: Configure in Program.cs

```csharp
using CMouss.IdentityFramework;
using CMouss.IdentityFramework.BlazorUI;
using CMouss.IdentityFramework.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Identity Framework
IDFManager.Configure(new IDFManagerConfig
{
    DatabaseType = DatabaseType.SQLite,
    DBConnectionString = "Data Source=identity.db;",
    AdministratorUserName = "admin",
    AdministratorPassword = "Admin@123",
    TokenEncryptionKey = "YourSecretKey123"
});

// Configure Blazor UI
IDFBlazorUIConfig.HomeURL = "/";
IDFBlazorUIConfig.AuthHomeURL = "/dashboard";
IDFBlazorUIConfig.LoginRedirectURL = "/login";
IDFBlazorUIConfig.SignupRedirectURL = "/signup";
IDFBlazorUIConfig.AfterLogoutRedirectURL = "/login";

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
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### Step 2: Add Component Imports

In `_Imports.razor`:

```razor
@using CMouss.IdentityFramework
@using CMouss.IdentityFramework.BlazorUI
@using CMouss.IdentityFramework.Models
```

## Configuration

### IDFBlazorUIConfig Class

Configure navigation and labels for the Blazor UI components.

```csharp
public static class IDFBlazorUIConfig
{
    // Navigation URLs
    public static string HomeURL { get; set; } = "";
    public static string AuthHomeURL { get; set; } = "";
    public static string LoginRedirectURL { get; set; } = "/login";
    public static string SignupRedirectURL { get; set; } = "/signup";
    public static string AfterLogoutRedirectURL { get; set; } = "/login";

    // Form Labels
    public static FormLabels FormLabels { get; set; } = new();
}
```

### FormLabels Class

Customize form labels and button text (useful for localization).

```csharp
public class FormLabels
{
    public string UserName { get; set; } = "Username";
    public string Password { get; set; } = "Password";
    public string LoginButton { get; set; } = "Login";
    public string SignupButton { get; set; } = "Sign Up";
    public string ResetPassword { get; set; } = "Reset Password";
}
```

### Configuration Example

```csharp
// Basic configuration
IDFBlazorUIConfig.AuthHomeURL = "/dashboard";
IDFBlazorUIConfig.LoginRedirectURL = "/login";

// Customize labels
IDFBlazorUIConfig.FormLabels.UserName = "Email Address";
IDFBlazorUIConfig.FormLabels.Password = "Your Password";
IDFBlazorUIConfig.FormLabels.LoginButton = "Sign In";

// Localization example (French)
IDFBlazorUIConfig.FormLabels = new FormLabels
{
    UserName = "Nom d'utilisateur",
    Password = "Mot de passe",
    LoginButton = "Se connecter",
    SignupButton = "S'inscrire"
};
```

## Components

### LoginPart

Pre-built login form component.

**Features**:
- Username/password input fields
- Automatic authentication
- Token storage in ProtectedLocalStorage
- Error message display
- Redirect after successful login
- Support for return URLs

**Usage**:

```razor
@page "/login"
@layout AuthEmptyLayout

<div class="login-container">
    <h2>Login</h2>
    <LoginPart />
</div>
```

**With Custom Styling**:

```razor
@page "/login"
@layout AuthEmptyLayout

<div class="auth-page">
    <div class="auth-card">
        <div class="auth-header">
            <h1>Welcome Back</h1>
            <p>Sign in to your account</p>
        </div>
        <LoginPart />
    </div>
</div>

<style>
    .auth-page {
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 100vh;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }

    .auth-card {
        background: white;
        padding: 2rem;
        border-radius: 8px;
        box-shadow: 0 10px 40px rgba(0,0,0,0.1);
        max-width: 400px;
        width: 100%;
    }
</style>
```

### SignUpPart

User registration component.

**Usage**:

```razor
@page "/signup"
@layout AuthEmptyLayout

<div class="signup-container">
    <h2>Create Account</h2>
    <SignUpPart />
</div>
```

### Logout

Logout functionality component.

**Usage**:

```razor
@page "/logout"

<Logout />
```

**Or as a link in navigation**:

```razor
<a href="/logout" class="logout-link">Logout</a>
```

## Layouts

### AuthLayout

Main authentication layout that protects pages and provides authentication state.

**Features**:
- Automatic token validation on page load
- Redirects to login if not authenticated
- Provides cascading authentication state
- Stores token securely using ProtectedLocalStorage
- Validates user session

**Usage**:

```razor
@page "/dashboard"
@layout AuthLayout

<h1>Welcome, @authLayoutModel.User.FullName!</h1>

<div>
    <p>Email: @authLayoutModel.User.Email</p>
    <p>User ID: @authLayoutModel.User.Id</p>
</div>

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }
}
```

### AuthLayoutModel

Cascading authentication state provided by AuthLayout.

```csharp
public class AuthLayoutModel
{
    public bool IsAuthenticated { get; set; }
    public User User { get; set; }
    public string Token { get; set; }
}
```

**Accessing in Components**:

```razor
@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }

    protected override void OnInitialized()
    {
        if (authLayoutModel.IsAuthenticated)
        {
            var userId = authLayoutModel.User.Id;
            var userName = authLayoutModel.User.UserName;
            var token = authLayoutModel.Token;
        }
    }
}
```

### AuthEmptyLayout

Empty layout for login/signup pages (no authentication required).

**Usage**:

```razor
@page "/login"
@layout AuthEmptyLayout

<div class="login-page">
    <LoginPart />
</div>
```

## Usage Examples

### Example 1: Basic Protected Page

```razor
@page "/profile"
@layout AuthLayout

<PageTitle>My Profile</PageTitle>

<h1>User Profile</h1>

<div class="profile-info">
    <div class="info-row">
        <label>Username:</label>
        <span>@authLayoutModel.User.UserName</span>
    </div>
    <div class="info-row">
        <label>Full Name:</label>
        <span>@authLayoutModel.User.FullName</span>
    </div>
    <div class="info-row">
        <label>Email:</label>
        <span>@authLayoutModel.User.Email</span>
    </div>
    <div class="info-row">
        <label>Account Status:</label>
        <span>@(authLayoutModel.User.IsActive ? "Active" : "Inactive")</span>
    </div>
</div>

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }
}
```

### Example 2: Dashboard with Data Loading

```razor
@page "/dashboard"
@layout AuthLayout
@inject NavigationManager Navigation

<PageTitle>Dashboard</PageTitle>

<h1>Welcome, @authLayoutModel.User.FullName!</h1>

@if (isLoading)
{
    <p>Loading dashboard data...</p>
}
else
{
    <div class="dashboard-grid">
        <div class="dashboard-card">
            <h3>Your Stats</h3>
            <p>Total Orders: @orderCount</p>
        </div>
        <div class="dashboard-card">
            <h3>Recent Activity</h3>
            <p>Last login: @authLayoutModel.User.LastIPAddress</p>
        </div>
    </div>
}

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }

    private bool isLoading = true;
    private int orderCount = 0;

    protected override async Task OnInitializedAsync()
    {
        // Load user-specific data using the token
        await LoadDashboardData();
        isLoading = false;
    }

    private async Task LoadDashboardData()
    {
        // Use authLayoutModel.Token to call APIs
        // Use authLayoutModel.User.Id for database queries

        // Example: Get order count
        // orderCount = await OrderService.GetUserOrderCount(authLayoutModel.User.Id);
    }
}
```

### Example 3: Role-Based Content Display

```razor
@page "/admin"
@layout AuthLayout

<PageTitle>Admin Panel</PageTitle>

@if (isAdmin)
{
    <h1>Admin Panel</h1>

    <div class="admin-tools">
        <button @onclick="ManageUsers">Manage Users</button>
        <button @onclick="ViewReports">View Reports</button>
        <button @onclick="SystemSettings">System Settings</button>
    </div>
}
else
{
    <div class="access-denied">
        <h2>Access Denied</h2>
        <p>You do not have permission to access this page.</p>
        <a href="/dashboard">Return to Dashboard</a>
    </div>
}

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }

    private bool isAdmin = false;

    protected override void OnInitialized()
    {
        // Check if user has admin role
        isAdmin = IDFManager.userService.ValidateUserRole(
            authLayoutModel.User.Id,
            "Administrators"
        );
    }

    private void ManageUsers()
    {
        // Navigate to user management
    }

    private void ViewReports()
    {
        // Navigate to reports
    }

    private void SystemSettings()
    {
        // Navigate to settings
    }
}
```

### Example 4: Permission-Based UI

```razor
@page "/orders"
@layout AuthLayout

<PageTitle>Orders</PageTitle>

<h1>Order Management</h1>

<div class="order-list">
    @foreach (var order in orders)
    {
        <div class="order-card">
            <h3>Order #@order.Id</h3>
            <p>Total: $@order.Total</p>

            @if (canUpdate)
            {
                <button @onclick="() => EditOrder(order.Id)">Edit</button>
            }

            @if (canDelete)
            {
                <button @onclick="() => DeleteOrder(order.Id)">Delete</button>
            }
        </div>
    }
</div>

@if (canCreate)
{
    <button @onclick="CreateNewOrder" class="btn-primary">Create New Order</button>
}

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }

    private List<Order> orders = new();
    private bool canCreate = false;
    private bool canUpdate = false;
    private bool canDelete = false;

    protected override void OnInitialized()
    {
        // Check permissions
        canCreate = IDFManager.userService.ValidateUserPermission(
            authLayoutModel.User.Id, "Order", "Create"
        );
        canUpdate = IDFManager.userService.ValidateUserPermission(
            authLayoutModel.User.Id, "Order", "Update"
        );
        canDelete = IDFManager.userService.ValidateUserPermission(
            authLayoutModel.User.Id, "Order", "Delete"
        );

        LoadOrders();
    }

    private void LoadOrders()
    {
        // Load orders for the user
    }

    private void CreateNewOrder()
    {
        // Navigate to order creation
    }

    private void EditOrder(string orderId)
    {
        // Navigate to order edit
    }

    private void DeleteOrder(string orderId)
    {
        // Delete order logic
    }

    public class Order
    {
        public string Id { get; set; }
        public decimal Total { get; set; }
    }
}
```

### Example 5: Custom Login Page with Branding

```razor
@page "/login"
@layout AuthEmptyLayout

<div class="login-page">
    <div class="login-container">
        <div class="brand-section">
            <img src="/images/logo.png" alt="Company Logo" class="logo" />
            <h1>Welcome to MyApp</h1>
            <p>Sign in to continue</p>
        </div>

        <div class="login-form-section">
            <LoginPart />
        </div>

        <div class="login-footer">
            <p>Don't have an account? <a href="/signup">Sign up here</a></p>
            <p><a href="/forgot-password">Forgot your password?</a></p>
        </div>
    </div>
</div>

<style>
    .login-page {
        min-height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
    }

    .login-container {
        background: white;
        border-radius: 12px;
        box-shadow: 0 20px 60px rgba(0,0,0,0.3);
        padding: 3rem;
        max-width: 450px;
        width: 100%;
    }

    .brand-section {
        text-align: center;
        margin-bottom: 2rem;
    }

    .logo {
        width: 120px;
        margin-bottom: 1rem;
    }

    .brand-section h1 {
        font-size: 1.8rem;
        color: #333;
        margin-bottom: 0.5rem;
    }

    .brand-section p {
        color: #666;
    }

    .login-footer {
        margin-top: 2rem;
        text-align: center;
        font-size: 0.9rem;
    }

    .login-footer a {
        color: #2a5298;
        text-decoration: none;
    }

    .login-footer a:hover {
        text-decoration: underline;
    }
</style>
```

### Example 6: Main Layout with Navigation

```razor
@inherits LayoutComponentBase
@layout AuthLayout

<div class="page">
    <div class="sidebar">
        <div class="sidebar-header">
            <h3>MyApp</h3>
        </div>

        <nav class="sidebar-nav">
            <a href="/dashboard" class="nav-item">
                <span class="icon">📊</span>
                Dashboard
            </a>
            <a href="/profile" class="nav-item">
                <span class="icon">👤</span>
                Profile
            </a>
            <a href="/orders" class="nav-item">
                <span class="icon">📦</span>
                Orders
            </a>

            @if (IsAdmin())
            {
                <a href="/admin" class="nav-item">
                    <span class="icon">⚙️</span>
                    Admin Panel
                </a>
            }
        </nav>
    </div>

    <main class="main-content">
        <div class="top-bar">
            <div class="user-info">
                <span>@authLayoutModel.User.FullName</span>
                <a href="/logout" class="logout-link">Logout</a>
            </div>
        </div>

        <article class="content">
            @Body
        </article>
    </main>
</div>

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }

    private bool IsAdmin()
    {
        return IDFManager.userService.ValidateUserRole(
            authLayoutModel.User.Id,
            "Administrators"
        );
    }
}

<style>
    .page {
        display: flex;
        height: 100vh;
    }

    .sidebar {
        width: 250px;
        background: #2c3e50;
        color: white;
    }

    .sidebar-header {
        padding: 1.5rem;
        border-bottom: 1px solid rgba(255,255,255,0.1);
    }

    .sidebar-nav {
        padding: 1rem 0;
    }

    .nav-item {
        display: flex;
        align-items: center;
        padding: 0.75rem 1.5rem;
        color: white;
        text-decoration: none;
        transition: background 0.2s;
    }

    .nav-item:hover {
        background: rgba(255,255,255,0.1);
    }

    .icon {
        margin-right: 0.75rem;
    }

    .main-content {
        flex: 1;
        display: flex;
        flex-direction: column;
        overflow: hidden;
    }

    .top-bar {
        background: white;
        padding: 1rem 2rem;
        border-bottom: 1px solid #e0e0e0;
        display: flex;
        justify-content: flex-end;
    }

    .user-info {
        display: flex;
        align-items: center;
        gap: 1rem;
    }

    .logout-link {
        color: #e74c3c;
        text-decoration: none;
    }

    .content {
        padding: 2rem;
        overflow-y: auto;
        flex: 1;
    }
</style>
```

## Customization

### Custom Form Labels

```csharp
// In Program.cs or Startup
IDFBlazorUIConfig.FormLabels = new FormLabels
{
    UserName = "Email or Username",
    Password = "Your Password",
    LoginButton = "Sign In",
    SignupButton = "Create Account",
    ResetPassword = "Forgot Password?"
};
```

### Custom Navigation Flow

```csharp
// Redirect to different pages after login/logout
IDFBlazorUIConfig.AuthHomeURL = "/home";
IDFBlazorUIConfig.AfterLogoutRedirectURL = "/";

// Use different login/signup page URLs
IDFBlazorUIConfig.LoginRedirectURL = "/auth/login";
IDFBlazorUIConfig.SignupRedirectURL = "/auth/register";
```

### Extending Components

Create custom components that use the framework:

```razor
@* CustomAuthHeader.razor *@
@inject NavigationManager Navigation

<div class="auth-header">
    @if (authLayoutModel.IsAuthenticated)
    {
        <div class="user-menu">
            <img src="/api/avatar/@authLayoutModel.User.Id" alt="Avatar" />
            <span>@authLayoutModel.User.FullName</span>
            <button @onclick="Logout">Logout</button>
        </div>
    }
    else
    {
        <div class="auth-buttons">
            <button @onclick="GoToLogin">Login</button>
            <button @onclick="GoToSignup">Sign Up</button>
        </div>
    }
</div>

@code {
    [CascadingParameter]
    public AuthLayoutModel authLayoutModel { get; set; }

    private void Logout()
    {
        Navigation.NavigateTo("/logout");
    }

    private void GoToLogin()
    {
        Navigation.NavigateTo(IDFBlazorUIConfig.LoginRedirectURL);
    }

    private void GoToSignup()
    {
        Navigation.NavigateTo(IDFBlazorUIConfig.SignupRedirectURL);
    }
}
```

## Best Practices

### 1. Always Use AuthLayout for Protected Pages

```razor
@* Correct *@
@page "/dashboard"
@layout AuthLayout

@* Wrong - no authentication *@
@page "/dashboard"
@layout MainLayout
```

### 2. Check Permissions Before Displaying Actions

```razor
@if (IDFManager.userService.ValidateUserPermission(authLayoutModel.User.Id, "Order", "Delete"))
{
    <button @onclick="DeleteOrder">Delete</button>
}
```

### 3. Handle Loading States

```razor
@if (authLayoutModel == null || !authLayoutModel.IsAuthenticated)
{
    <p>Loading...</p>
}
else
{
    <h1>Welcome, @authLayoutModel.User.FullName</h1>
}
```

### 4. Secure Sensitive Operations

```razor
@code {
    private async Task DeleteUser(string userId)
    {
        // Re-validate permission before critical operations
        bool canDelete = IDFManager.userService.ValidateUserPermission(
            authLayoutModel.User.Id,
            "User",
            "Delete"
        );

        if (!canDelete)
        {
            // Show error message
            return;
        }

        // Proceed with delete
    }
}
```

### 5. Use ReturnUrl for Login Redirects

```razor
@* In a protected page that detects unauthorized access *@
@code {
    protected override void OnInitialized()
    {
        if (!authLayoutModel.IsAuthenticated)
        {
            var returnUrl = Navigation.Uri;
            Navigation.NavigateTo($"/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
        }
    }
}
```

### 6. Clear Sensitive Data on Logout

The framework automatically clears tokens from ProtectedLocalStorage on logout.

---

[← Back to README](README.md) | [API Server Documentation →](CMouss.IdentityFramework.APIServer.md)
