using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class AppConverter
    {
        public static APIModels.App ToAPIApp(DBModels.App dbApp, bool loadOwner, bool loadPersmissionTypes)
        {
            APIModels.App apiApp = new();
            apiApp.Id = dbApp.Id;
            apiApp.Title = dbApp.Title;
            apiApp.IsActive = dbApp.IsActive;

            if (loadOwner)
            {
                APIModels.User apiUser = new();
                apiUser.Id = dbApp.OwnerId;
                if (dbApp.Owner is not null)
                {
                    apiApp.Owner = UserConverter.ToAPIUser(dbApp.Owner, false, false, null);
                }
            }
            //if (loadPersmissionTypes && apiApp.AppPermissionTypes.Count> 0)
            //{
            //    apiApp.AppPermissionTypes = AppPermissionTypeConverter.ToAPIAppPermissionTypesList(dbApp.);
                  
            //}

            return apiApp;
        }

        public static List<APIModels.App> ToAPIAppsList(List<DBModels.App> dbApps)
        {
            List<APIModels.App> appiApps = new();
            foreach (DBModels.App d in dbApps)
            {
                APIModels.App a = ToAPIApp(d, true, false) ;
                appiApps.Add(a);
            }
            return appiApps;
        }
    }
}
