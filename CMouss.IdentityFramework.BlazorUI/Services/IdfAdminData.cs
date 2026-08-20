using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CMouss.IdentityFramework.BlazorUI.Services
{
    /// <summary>
    /// Data facade used by the Identity Framework management components.
    ///
    /// Everything goes through the CMouss.IdentityFramework services (IDFManager.userService,
    /// IDFManager.roleService, ...). This class only:
    ///   - exposes the four services that IDFManager does not publish as a static instance
    ///     (AppService, AppAppPermissionTypeService, AppAccessPermissionService, AttributeTypeService),
    ///   - adds the read queries the lists need (tokens of a user, accesses of an app, ...),
    ///   - performs the delete operations, because Delete(...) is still a "//TODO: Delete X" stub
    ///     in EntityService, PermissionTypeService, PermissionService, RoleService, AppService,
    ///     AppPermissionTypeService, AppAccessPermissionService and AttributeTypeService
    ///     (they call SaveChanges without removing anything), and because
    ///     UserTokenService.Delete(token) removes the row without calling SaveChanges.
    ///     Once those stubs are implemented in the core project, the Delete* methods below can
    ///     simply forward to the matching core service.
    /// </summary>
    public static class IdfAdminData
    {
        #region Services not published by IDFManager

        public static AppService AppService { get; } = new AppService();
        public static AppAppPermissionTypeService AppPermissionTypeService { get; } = new AppAppPermissionTypeService();
        public static AppAccessPermissionService AppAccessPermissionService { get; } = new AppAccessPermissionService();
        public static AttributeTypeService AttributeTypeService { get; } = new AttributeTypeService();

        private static IDFDBContext DB
        {
            get { return IDFManager.Context; }
        }

        #endregion

        #region Users

        public static List<User> SearchUsers(Paging paging, UsersSearch filter)
        {
            return IDFManager.userService.Search(paging, filter);
        }

        public static int CountUsers(UsersSearch filter)
        {
            return DB.Users.Count(o =>
                (string.IsNullOrEmpty(filter.UserName) || o.UserName.ToLower().Contains(filter.UserName.ToLower()))
                && (string.IsNullOrEmpty(filter.FullName) || o.FullName.ToLower().Contains(filter.FullName.ToLower()))
                && (string.IsNullOrEmpty(filter.Email) || o.Email.ToLower().Contains(filter.Email.ToLower()))
                && (filter.IsActive == null || filter.IsActive == o.IsActive)
                && (filter.IsDeleted == null || filter.IsDeleted == o.IsDeleted)
                && (filter.IsLocked == null || filter.IsLocked == o.IsLocked));
        }

        /// <summary>
        /// Returns the user with its roles, or null when it does not exist.
        /// </summary>
        public static User? FindUser(string id)
        {
            List<User> lst = DB.Users.Include(o => o.Roles).Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { return null; }
            return lst[0];
        }

        public static List<User> GetUsers(bool includeDeleted)
        {
            return IDFManager.userService.GetAll(includeDeleted);
        }

        public static List<Role> GetUserRoles(string userId)
        {
            return IDFManager.userService.GetRoles(userId);
        }

        public static List<EntityAllowedActionsModel> GetUserPermissions(string userId)
        {
            return IDFManager.userService.GetUserPermissions(userId);
        }

        #endregion

        #region Roles

        public static List<Role> GetRoles()
        {
            return DB.Roles.Include(o => o.Permissions).OrderBy(o => o.Title).ToList();
        }

        public static Role? FindRole(string id)
        {
            return IDFManager.roleService.Find(id);
        }

        public static List<User> GetRoleUsers(string roleId)
        {
            List<Role> lst = DB.Roles.Include(o => o.Users).Where(o => o.Id == roleId).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.RoleNotFound); }
            return lst[0].Users ?? new List<User>();
        }

        public static List<Permission> GetRolePermissions(string roleId)
        {
            return DB.Permissions
                .Include(o => o.Entity)
                .Include(o => o.PermissionType)
                .Where(o => o.RoleId == roleId).ToList();
        }

        /// <summary>
        /// Deletes a role, its permissions and all of its user assignments.
        /// The administrators role is protected.
        /// </summary>
        public static void DeleteRole(string id)
        {
            if (!string.IsNullOrEmpty(IDFManager.AdministratorRoleId)
                && string.Equals(id, IDFManager.AdministratorRoleId, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("The administrators role cannot be deleted");
            }

            List<Role> lst = DB.Roles.Include(o => o.Users).Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.RoleNotFound); }

            DB.Permissions.RemoveRange(DB.Permissions.Where(o => o.RoleId == id).ToList());
            DB.RoleUsers.RemoveRange(DB.RoleUsers.Where(o => o.RoleId == id).ToList());
            if (lst[0].Users is not null) { lst[0].Users.Clear(); }

            DB.Roles.Remove(lst[0]);
            DB.SaveChanges();
        }

        #endregion

        #region Entities

        public static List<Entity> GetEntities()
        {
            return DB.Entities.OrderBy(o => o.Id).ToList();
        }

        /// <summary>
        /// Deletes an entity and every permission granted on it.
        /// </summary>
        public static void DeleteEntity(string id)
        {
            List<Entity> lst = DB.Entities.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.NotFound); }

            DB.Permissions.RemoveRange(DB.Permissions.Where(o => o.EntityId == id).ToList());
            DB.Entities.Remove(lst[0]);
            DB.SaveChanges();
        }

        #endregion

        #region Permission types (actions)

        public static List<PermissionType> GetPermissionTypes()
        {
            return DB.PermissionTypes.OrderBy(o => o.Id).ToList();
        }

        /// <summary>
        /// Deletes an action and every permission using it.
        /// </summary>
        public static void DeletePermissionType(string id)
        {
            List<PermissionType> lst = DB.PermissionTypes.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.NotFound); }

            DB.Permissions.RemoveRange(DB.Permissions.Where(o => o.PermissionTypeId == id).ToList());
            DB.PermissionTypes.Remove(lst[0]);
            DB.SaveChanges();
        }

        #endregion

        #region Permissions (Role + Entity + Action)

        public static List<Permission> GetPermissions(string? roleId, string? entityId, string? permissionTypeId)
        {
            return DB.Permissions
                .Include(o => o.Entity)
                .Include(o => o.PermissionType)
                .Include(o => o.Role)
                .Where(o =>
                    (string.IsNullOrEmpty(roleId) || o.RoleId == roleId)
                    && (string.IsNullOrEmpty(entityId) || o.EntityId == entityId)
                    && (string.IsNullOrEmpty(permissionTypeId) || o.PermissionTypeId == permissionTypeId))
                .ToList();
        }

        public static bool PermissionExists(string roleId, string entityId, string permissionTypeId)
        {
            return DB.Permissions.Any(o =>
                o.RoleId == roleId
                && o.EntityId == entityId
                && o.PermissionTypeId == permissionTypeId);
        }

        /// <summary>
        /// Grants Role + Entity + Action, does nothing when it is already granted.
        /// </summary>
        public static void GrantPermission(string roleId, string entityId, string permissionTypeId)
        {
            if (PermissionExists(roleId, entityId, permissionTypeId)) { return; }
            IDFManager.permissionService.Create(Helpers.GenerateId(), roleId, entityId, permissionTypeId);
        }

        /// <summary>
        /// Revokes Role + Entity + Action, does nothing when it is not granted.
        /// </summary>
        public static void RevokePermission(string roleId, string entityId, string permissionTypeId)
        {
            List<Permission> lst = DB.Permissions.Where(o =>
                o.RoleId == roleId
                && o.EntityId == entityId
                && o.PermissionTypeId == permissionTypeId).ToList();
            if (lst.Count == 0) { return; }
            DB.Permissions.RemoveRange(lst);
            DB.SaveChanges();
        }

        public static void DeletePermission(string id)
        {
            List<Permission> lst = DB.Permissions.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.NotFound); }
            DB.Permissions.Remove(lst[0]);
            DB.SaveChanges();
        }

        #endregion

        #region Apps

        public static List<App> GetApps(bool includeDeleted)
        {
            return DB.Apps.Include(o => o.Owner)
                .Where(o => includeDeleted || o.IsDeleted == false)
                .OrderBy(o => o.Title).ToList();
        }

        /// <summary>
        /// Returns the app whatever its active / deleted state is, or null when it does not exist.
        /// AppService.Details only returns active apps, which hides a freshly created one.
        /// </summary>
        public static App? FindApp(string id)
        {
            List<App> lst = DB.Apps.Include(o => o.Owner).Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { return null; }
            return lst[0];
        }

        public static void ChangeAppOwner(string id, string ownerId)
        {
            List<App> lst = DB.Apps.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception("App not found"); }
            if (!DB.Users.Any(o => o.Id == ownerId)) { throw new Exception(Messages.UserNotFound); }
            lst[0].OwnerId = ownerId;
            DB.SaveChanges();
        }

        /// <summary>
        /// Soft deletes the app (IsDeleted), deactivates it and revokes all of its accesses.
        /// </summary>
        public static void DeleteApp(string id)
        {
            List<App> lst = DB.Apps.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception("App not found"); }

            List<AppAccess> accesses = DB.AppAccess.Where(o => o.AppId == id).ToList();
            List<string> accessIds = accesses.Select(o => o.Id).ToList();
            DB.AppAccessPermissions.RemoveRange(DB.AppAccessPermissions.Where(o => accessIds.Contains(o.AppAccessId!)).ToList());
            DB.AppAccess.RemoveRange(accesses);

            lst[0].IsDeleted = true;
            lst[0].IsActive = false;
            DB.SaveChanges();
        }

        public static void RestoreApp(string id)
        {
            List<App> lst = DB.Apps.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception("App not found"); }
            lst[0].IsDeleted = false;
            DB.SaveChanges();
        }

        /// <summary>
        /// Permanently removes the app, its permission types and all of its accesses.
        /// </summary>
        public static void HardDeleteApp(string id)
        {
            List<App> lst = DB.Apps.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception("App not found"); }

            List<AppAccess> accesses = DB.AppAccess.Where(o => o.AppId == id).ToList();
            List<string> accessIds = accesses.Select(o => o.Id).ToList();
            List<AppPermissionType> permissionTypes = DB.AppPermissionTypes.Where(o => o.AppId == id).ToList();
            List<string> permissionTypeIds = permissionTypes.Select(o => o.Id).ToList();

            DB.AppAccessPermissions.RemoveRange(DB.AppAccessPermissions
                .Where(o => accessIds.Contains(o.AppAccessId!) || permissionTypeIds.Contains(o.AppPermissionTypeId)).ToList());
            DB.AppAccess.RemoveRange(accesses);
            DB.AppPermissionTypes.RemoveRange(permissionTypes);
            DB.Apps.Remove(lst[0]);
            DB.SaveChanges();
        }

        #endregion

        #region App permission types

        public static List<AppPermissionType> GetAppPermissionTypes(string? appId)
        {
            return DB.AppPermissionTypes.Include(o => o.App)
                .Where(o => string.IsNullOrEmpty(appId) || o.AppId == appId)
                .OrderBy(o => o.Id).ToList();
        }

        /// <summary>
        /// Deletes an app permission type and revokes it from every access holding it.
        /// </summary>
        public static void DeleteAppPermissionType(string id)
        {
            List<AppPermissionType> lst = DB.AppPermissionTypes.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.AppPermissionTypeNotFound); }

            DB.AppAccessPermissions.RemoveRange(DB.AppAccessPermissions.Where(o => o.AppPermissionTypeId == id).ToList());
            DB.AppPermissionTypes.Remove(lst[0]);
            DB.SaveChanges();
        }

        #endregion

        #region App accesses (app tokens)

        public static List<AppAccess> GetAppAccesses(string? appId, string? userId)
        {
            return DB.AppAccess
                .Include(o => o.App).ThenInclude(o => o.Owner)
                .Include(o => o.User)
                .Include(o => o.AppAccessPermissions).ThenInclude(o => o.AppPermissionType)
                .Where(o =>
                    (string.IsNullOrEmpty(appId) || o.AppId == appId)
                    && (string.IsNullOrEmpty(userId) || o.UserId == userId))
                .OrderByDescending(o => o.ExpireDate)
                .ToList();
        }

        public static AppAccess? FindAppAccess(string id)
        {
            List<AppAccess> lst = DB.AppAccess
                .Include(o => o.App).ThenInclude(o => o.Owner)
                .Include(o => o.User)
                .Include(o => o.AppAccessPermissions).ThenInclude(o => o.AppPermissionType)
                .Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { return null; }
            return lst[0];
        }

        /// <summary>
        /// Generates a new AccessSecret and a new expiry date for the given access.
        /// </summary>
        public static void RefreshAppAccessSecret(string id, LifeTime lifeTime)
        {
            List<AppAccess> lst = DB.AppAccess.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.AppAccessNotFound); }
            lst[0].AccessSecret = Helpers.GenerateKey();
            lst[0].ExpireDate = DateTime.Now.AddDays(lifeTime.Days).AddHours(lifeTime.Hours).AddMinutes(lifeTime.Minutes);
            DB.SaveChanges();
        }

        /// <summary>
        /// Pushes back the expiry date without changing the secret.
        /// </summary>
        public static void ExtendAppAccess(string id, LifeTime lifeTime)
        {
            List<AppAccess> lst = DB.AppAccess.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.AppAccessNotFound); }
            lst[0].ExpireDate = DateTime.Now.AddDays(lifeTime.Days).AddHours(lifeTime.Hours).AddMinutes(lifeTime.Minutes);
            DB.SaveChanges();
        }

        /// <summary>
        /// Sets the comma separated list of IPs allowed to use the access, an empty value allows any IP.
        /// </summary>
        public static void SetAppAccessAllowedIPs(string id, string ipAddresses)
        {
            List<AppAccess> lst = DB.AppAccess.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.AppAccessNotFound); }
            lst[0].AllowedIPAddresses = ipAddresses ?? "";
            DB.SaveChanges();
        }

        public static void DeleteAppAccess(string id)
        {
            List<AppAccess> lst = DB.AppAccess.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.AppAccessNotFound); }

            DB.AppAccessPermissions.RemoveRange(DB.AppAccessPermissions.Where(o => o.AppAccessId == id).ToList());
            DB.AppAccess.Remove(lst[0]);
            DB.SaveChanges();
        }

        public static List<AppAccessPermission> GetAppAccessPermissions(string appAccessId)
        {
            return DB.AppAccessPermissions
                .Include(o => o.AppPermissionType)
                .Where(o => o.AppAccessId == appAccessId).ToList();
        }

        /// <summary>
        /// Revokes an app permission from an app access.
        /// AppAccessService.RevokePermission does the same, this version does not throw when
        /// the permission is not granted.
        /// </summary>
        public static void RevokeAppAccessPermission(string appAccessId, string appPermissionTypeId)
        {
            List<AppAccessPermission> lst = DB.AppAccessPermissions.Where(o =>
                o.AppAccessId == appAccessId
                && o.AppPermissionTypeId == appPermissionTypeId).ToList();
            if (lst.Count == 0) { return; }
            DB.AppAccessPermissions.RemoveRange(lst);
            DB.SaveChanges();
        }

        #endregion

        #region User tokens

        public static List<UserToken> GetUserTokens(string? userId, bool includeExpired)
        {
            return DB.UserTokens.Include(o => o.User)
                .Where(o =>
                    (string.IsNullOrEmpty(userId) || o.UserId == userId)
                    && (includeExpired || o.ExpireDate >= DateTime.Now))
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        public static void DeleteUserToken(long id)
        {
            List<UserToken> lst = DB.UserTokens.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.NotFound); }
            DB.UserTokens.Remove(lst[0]);
            DB.SaveChanges();
        }

        #endregion

        #region Attribute types

        public static List<AttributeType> GetAttributeTypes()
        {
            return DB.AttributeTypes.OrderBy(o => o.Id).ToList();
        }

        /// <summary>
        /// Deletes an attribute type and every user value stored for it.
        /// </summary>
        public static void DeleteAttributeType(string id)
        {
            List<AttributeType> lst = DB.AttributeTypes.Where(o => o.Id == id).ToList();
            if (lst.Count == 0) { throw new Exception(Messages.NotFound); }

            DB.AttributeItems.RemoveRange(DB.AttributeItems.Where(o => o.AttributeTypeId == id).ToList());
            DB.AttributeTypes.Remove(lst[0]);
            DB.SaveChanges();
        }

        #endregion
    }
}
