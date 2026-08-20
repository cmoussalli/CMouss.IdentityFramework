using System;
using System.Collections.Generic;
using System.Linq;

namespace CMouss.IdentityFramework.BlazorUI
{
    /// <summary>
    /// Configuration of the Identity Framework management components (users, roles, apps, tokens...).
    /// Set it once at startup, next to IDFBlazorUIConfig.
    /// </summary>
    public static class IDFBlazorUIAdminConfig
    {
        /// <summary>
        /// Roles allowed to open the management components.
        /// When left empty, IDFManager.AdministratorRoleId is used.
        /// </summary>
        public static List<string> AdminRoleIds { get; set; } = new();

        /// <summary>
        /// Number of rows loaded per page in the management lists.
        /// </summary>
        public static int PageSize { get; set; } = 25;

        /// <summary>
        /// When false, every destructive action (delete / revoke / purge) is hidden.
        /// </summary>
        public static bool AllowDeleteOperations { get; set; } = true;

        /// <summary>
        /// When false, the generated AccessSecret and the user tokens are always masked.
        /// </summary>
        public static bool AllowRevealSecrets { get; set; } = true;

        /// <summary>
        /// Default lifetime proposed when generating an App Access.
        /// When null, IDFManager.AppAccessDefaultLifeTime is used.
        /// </summary>
        public static LifeTime? DefaultAppAccessLifeTime { get; set; } = null;

        /// <summary>
        /// Default lifetime proposed when creating a user token.
        /// When null, IDFManager.TokenDefaultLifeTime is used.
        /// </summary>
        public static LifeTime? DefaultUserTokenLifeTime { get; set; } = null;

        /// <summary>
        /// Returns the roles allowed to manage the identity framework.
        /// </summary>
        public static List<string> GetAdminRoleIds()
        {
            if (AdminRoleIds is not null && AdminRoleIds.Count > 0)
            {
                return AdminRoleIds;
            }
            List<string> result = new();
            if (!String.IsNullOrEmpty(IDFManager.AdministratorRoleId))
            {
                result.Add(IDFManager.AdministratorRoleId);
            }
            return result;
        }

        public static LifeTime GetDefaultAppAccessLifeTime()
        {
            return DefaultAppAccessLifeTime ?? IDFManager.AppAccessDefaultLifeTime ?? new LifeTime(30, 0, 0);
        }

        public static LifeTime GetDefaultUserTokenLifeTime()
        {
            return DefaultUserTokenLifeTime ?? IDFManager.TokenDefaultLifeTime ?? new LifeTime(1, 0, 0);
        }
    }
}
