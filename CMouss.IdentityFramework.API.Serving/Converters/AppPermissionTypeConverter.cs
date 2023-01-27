using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class AppPermissionTypeConverter
    {
        public static APIModels.AppPermissionType ToAPIAppPermissionType(DBModels.AppPermissionType dbAppPermissionType)
        {
            APIModels.AppPermissionType apiAppPermissionType = new();
            apiAppPermissionType.Id = dbAppPermissionType.Id;
            apiAppPermissionType.Title = dbAppPermissionType.Title;
            apiAppPermissionType.App = Converters.AppConverter.ToAPIApp(dbAppPermissionType.App,false,false);
            return apiAppPermissionType;
        }

        public static List<APIModels.AppPermissionType> ToAPIAppPermissionTypesList(List<DBModels.AppPermissionType> dbAppPermissionTypes)
        {
            List<APIModels.AppPermissionType> apiAppPermissionTypes = new();
            foreach(DBModels.AppPermissionType d in dbAppPermissionTypes)
            {
                APIModels.AppPermissionType a = ToAPIAppPermissionType(d);
                apiAppPermissionTypes.Add(a);
            }
            return apiAppPermissionTypes;
        }
    }
}
