using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class PermissionConverter
    {
        public static APIModels.Permission ToAPIPermission(DBModels.Permission dbPermission)
        {
            APIModels.Permission apiPermission = new();
            apiPermission.Id = dbPermission.Id;
            apiPermission.Entity = Converters.EntityConverter.ToAPIEntity(dbPermission.Entity);
            apiPermission.PermissionType = Converters.PermissionTypeConverter.ToAPIPermissionType(dbPermission.PermissionType);

            if (dbPermission.Role is not null)
            {
                apiPermission.Role = RoleConverter.ToAPIRole(dbPermission.Role, false,false) ;
            }

            return apiPermission;
        }

        public static List<APIModels.Permission> ToAPIPermissionsList(List<DBModels.Permission> dbPermissions)
        {
            List<APIModels.Permission> PermissioniPermissions = new();
            foreach (DBModels.Permission d in dbPermissions)
            {
                APIModels.Permission a = ToAPIPermission(d);
                PermissioniPermissions.Add(a);
            }
            return PermissioniPermissions;
        }
    }
}
