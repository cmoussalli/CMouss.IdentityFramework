using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class RoleConverter
    {
        public static APIModels.Role ToAPIRole(DBModels.Role dbRole,bool loadPermissions, bool loadUsers)
        {
            APIModels.Role apiRole = new();
            apiRole.Id = dbRole.Id;
            apiRole.Title = dbRole.Title;
            if (loadPermissions) { apiRole.Permissions = PermissionConverter.ToAPIPermissionsList(dbRole.Permissions); }
            return apiRole;
        }

        public static List<APIModels.Role> ToAPIRolesList(List<DBModels.Role> dbRoles,bool loadPermissions, bool loadUsers)
        {
            List<APIModels.Role> RoleiRoles = new();
            foreach (DBModels.Role d in dbRoles)
            {
                APIModels.Role a = ToAPIRole(d,loadPermissions,loadUsers);
                RoleiRoles.Add(a);
            }
            return RoleiRoles;
        }
    }
}
