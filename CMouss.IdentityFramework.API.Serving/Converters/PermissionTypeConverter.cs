using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class PermissionTypeConverter
    {
        public static APIModels.PermissionType ToAPIPermissionType(DBModels.PermissionType dbPermissionType)
        {
            APIModels.PermissionType apiPermissionType = new();
            apiPermissionType.Id = dbPermissionType.Id;
            apiPermissionType.Title = dbPermissionType.Title;
            return apiPermissionType;
        }

        public static List<APIModels.PermissionType> ToAPIPermissionTypesList(List<DBModels.PermissionType> dbPermissionTypes)
        {
            List<APIModels.PermissionType> PermissionTypeiPermissionTypes = new();
            foreach(DBModels.PermissionType d in dbPermissionTypes)
            {
                APIModels.PermissionType a = ToAPIPermissionType(d);
                PermissionTypeiPermissionTypes.Add(a);
            }
            return PermissionTypeiPermissionTypes;
        }
    }
}
