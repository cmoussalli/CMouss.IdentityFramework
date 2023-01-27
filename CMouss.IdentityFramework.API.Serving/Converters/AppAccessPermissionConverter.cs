using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class AppAccessPermissionConverter
    {
        public static APIModels.AppAccessPermission ToAPIAppAccessPermission(DBModels.AppAccessPermission dbAppAccessPermission)
        {
            APIModels.AppAccessPermission appAccessPermission = new();
            appAccessPermission.Id = dbAppAccessPermission.Id;
            appAccessPermission.AppPermissionType = Converters.AppPermissionTypeConverter.ToAPIAppPermissionType(dbAppAccessPermission.AppPermissionType);
            appAccessPermission.AppAccess =  Converters.AppAccessConverter.ToAPIAppAccess( dbAppAccessPermission.AppAccess);
            return appAccessPermission;
        }

        public static List<APIModels.AppAccessPermission> ToAPIAppAccessPermissionList(List<DBModels.AppAccessPermission> dbAppAccessPermission)
        {
            List<APIModels.AppAccessPermission> appAccessPermissions = new();
            foreach (DBModels.AppAccessPermission d in dbAppAccessPermission)
            {
                APIModels.AppAccessPermission a = ToAPIAppAccessPermission(d);
                appAccessPermissions.Add(a);
            }
            return appAccessPermissions;
        }
    }
}
