# Blazor UI — Management components

Beyond the login / signup / logout parts, `CMouss.IdentityFramework.BlazorUI` ships a full
management UI covering every function of `CMouss.IdentityFramework` that can be driven from a screen.

All of it is packaged as **components**, not routed pages, so you decide where they live in your app.

## Table of contents

- [Quick start](#quick-start)
- [Configuration](#configuration)
- [Components](#components)
- [Authorization](#authorization)
- [Requirements](#requirements)
- [What is covered](#what-is-covered)
- [Known limitations](#known-limitations)

## Quick start

One component gives you everything, in tabs:

```razor
@page "/admin"
@layout MainLayoutAuth

<CMouss.IdentityFramework.BlazorUI.Admin.IdentityAdminPart DefaultTab="users" />
```

And the self service screen for the signed in user:

```razor
@page "/profile"
@layout MainLayoutAuth

<CMouss.IdentityFramework.BlazorUI.Account.MyProfilePart />
```

`MainLayoutAuth` is your own layout wrapping `<AuthLayout>`, exactly like the existing demo:

```razor
@inherits LayoutComponentBase

<AuthLayout>
    <div class="page">
        <main><article class="content px-4">@Body</article></main>
    </div>
</AuthLayout>
```

Each screen can also be used on its own, so you can spread them across your own menu:

```razor
<UsersManagerPart />
<AppAccessManagerPart AppId="@myAppId" />
```

## Configuration

Set this once at startup, next to `IDFBlazorUIConfig`:

```csharp
using CMouss.IdentityFramework.BlazorUI;

// Roles allowed to open the management screens.
// Empty (default) means IDFManager.AdministratorRoleId is used.
IDFBlazorUIAdminConfig.AdminRoleIds = new List<string> { "Administrators", "IdentityManagers" };

IDFBlazorUIAdminConfig.PageSize = 25;               // rows per page in the user list
IDFBlazorUIAdminConfig.AllowDeleteOperations = true; // false hides every delete / revoke / purge button
IDFBlazorUIAdminConfig.AllowRevealSecrets = true;    // false keeps keys, secrets and tokens masked

// Lifetime proposed by default in the generate / issue dialogs.
// null falls back to IDFManager.AppAccessDefaultLifeTime / IDFManager.TokenDefaultLifeTime.
IDFBlazorUIAdminConfig.DefaultAppAccessLifeTime = new LifeTime(30, 0, 0);
IDFBlazorUIAdminConfig.DefaultUserTokenLifeTime = new LifeTime(1, 0, 0);
```

## Components

| Component | Namespace | What it manages |
|---|---|---|
| `IdentityAdminPart` | `...BlazorUI.Admin` | Tabbed shell hosting all the screens below |
| `UsersManagerPart` | `...BlazorUI.Admin` | Search, create, update, lock / unlock, delete users, grant / revoke roles, change password, view effective permissions |
| `RolesManagerPart` | `...BlazorUI.Admin` | Roles CRUD, permission matrix per role, users holding a role |
| `EntitiesManagerPart` | `...BlazorUI.Admin` | Entities CRUD |
| `PermissionTypesManagerPart` | `...BlazorUI.Admin` | Actions (permission types) CRUD |
| `PermissionsManagerPart` | `...BlazorUI.Admin` | Role × Entity × Action matrix, and a filterable list view |
| `AppsManagerPart` | `...BlazorUI.Admin` | Apps CRUD, owner, activate, soft delete / restore / purge |
| `AppPermissionTypesManagerPart` | `...BlazorUI.Admin` | App permission types CRUD (optionally scoped with `AppId`) |
| `AppAccessManagerPart` | `...BlazorUI.Admin` | Generate app accesses, refresh the secret, extend, allowed IPs, grant / revoke app permissions, delete, purge expired (scopeable with `AppId` / `UserId`) |
| `UserTokensManagerPart` | `...BlazorUI.Admin` | Issue tokens, list them, revoke one, revoke all of a user, purge expired (scopeable with `UserId`) |
| `AttributeTypesManagerPart` | `...BlazorUI.Admin` | Attribute types CRUD |
| `MaintenancePart` | `...BlazorUI.Admin` | Counters, read only configuration, access tester, cache / context reload, administrator creation, purges, tracked sessions |
| `MyProfilePart` | `...BlazorUI.Account` | Self service: profile, change my password, my permissions, my sessions |

Shared building blocks live in `...BlazorUI.Shared`: `IdfPanel`, `IdfModal`, `IdfConfirm`,
`IdfAlerts` and `IdfSecret` (masked value with reveal and copy).

Common parameters on every management component:

| Parameter | Default | Meaning |
|---|---|---|
| `RequireAdminRole` | `true` | Set `false` when the hosting page already checked the access |
| `OnChanged` | — | Raised after any successful change, to refresh the hosting page |

## Authorization

Each component resolves the signed in user by itself:

1. from the cascading `AuthLayoutModel` when hosted inside `<AuthLayout>`,
2. otherwise from the auth cookie through `CookieAuthService`.

It then checks the role against `IDFBlazorUIAdminConfig.GetAdminRoleIds()` using
`IDFManager.userService.ValidateUserRole` (the database is the reference; the token claims are only
used as a fallback). A user without the role sees an "Access denied" panel instead of the screen.

`MyProfilePart` only requires a signed in user, never an admin role.

## Requirements

- **Bootstrap 5** must be loaded by the host app (the demo already links it in `App.razor`).
  The markup uses plain Bootstrap classes only, no extra CSS or JS dependency, and no `confirm()` /
  `alert()` dialogs (confirmations are in page modals so the Blazor circuit is never blocked).
- The components must render **interactively** (`InteractiveServer`). They do not declare
  `@rendermode` themselves so that they inherit the render mode of the hosting page, which keeps the
  cascading `AuthLayoutModel` flowing. In the demo this comes from
  `<Routes @rendermode="InteractiveServer" />`.
- The component assembly must be added to the router, as it already is for the login parts:

```razor
<Router AppAssembly="typeof(Program).Assembly"
        AdditionalAssemblies="new[] { typeof(CMouss.IdentityFramework.BlazorUI.AssemblyMarker).Assembly }">
```

## What is covered

Everything the framework exposes that makes sense in a UI:

- **Users** — `Search`, `Create`, `Update`, `Delete`, `Lock`, `UnLock`, `ChangePassword`,
  `ChangeMyPassword`, `GetRoles`, `GrantRole`, `RevokeRole`, `GetUserPermissions`,
  `GetUserEntityPermissions`, `ValidateUserRole`, `ValidateUserPermission`
- **Roles** — `Find`, `GetAll`, `Create`, `Update`, delete, role users, role permissions
- **Entities / Permission types / Permissions** — full CRUD and the grant matrix
- **Apps** — `Create`, `Update`, owner change, activate, soft delete, restore, purge
- **App permission types** — `Create`, `Update`, delete
- **App accesses** — `Generate`, secret refresh, expiry extension, `SetAllowedIPs`,
  `GrantPermission`, `RevokePermission`, delete, `CleanExpiredAppAccesss`
- **User tokens** — `Create`, list, revoke, `DeleteUserTokens`, `CleanExpiredUserTokens`
- **Attribute types** — `Create`, `Update`, delete
- **Runtime** — `RefreshIDFStorage`, `RefreshIDFDBContext`, `CreateAdministrator`, `UserSessions`

Not exposed, because they are not UI operations: `AuthUserLogin` / `AuthUserToken*` (already used by
`LoginPart`), `Validate` on tokens and app accesses (used by the auth pipeline), and the LDAP
connection settings (startup configuration only).

## Known limitations

- `AttributeItem` values per user are not editable yet: the core framework has no service for them,
  only the `AttributeTypes` they are based on. The attribute *types* are fully manageable.
- With `TokenValidationMode.DecryptOnly`, revoking a stored token does not block it until it expires,
  since tokens are validated by decryption only. The token screens display this warning.
- Deleting a role or a permission invalidates the `Storage` cache used to resolve the roles of a
  claim. The screens call `IDFManager.RefreshIDFStorage()` after such a change, and the
  **System** tab offers the same action manually.
