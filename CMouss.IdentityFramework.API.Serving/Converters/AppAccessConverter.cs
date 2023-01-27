using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class AppAccessConverter
    {
        public static APIModels.AppAccess ToAPIAppAccess(DBModels.AppAccess dbAppAccess)
        {
            APIModels.AppAccess apiAppAccess = new();
            apiAppAccess.Id = dbAppAccess.Id;
            apiAppAccess.ExpireDate = dbAppAccess.ExpireDate;
            apiAppAccess.AccessKey = dbAppAccess.AccessKey;
            apiAppAccess.AccessSecret = dbAppAccess.AccessSecret;

            APIModels.App apiApp = new();
            apiApp.Id = dbAppAccess.AppId;
            if (dbAppAccess.App is not null)
            {
                apiAppAccess.App = AppConverter.ToAPIApp(dbAppAccess.App,false,false);
            }

            APIModels.User apiUser = new();
            apiUser.Id = dbAppAccess.UserId;
            if (dbAppAccess.User is not null)
            {
                apiAppAccess.User = UserConverter.ToAPIUser(dbAppAccess.User, false, false,null);
            }

            return apiAppAccess;
        }

        public static List<APIModels.AppAccess> ToAPIAppAccesssList(List<DBModels.AppAccess> dbAppAccesss)
        {
            List<APIModels.AppAccess> AppAccessiAppAccesss = new();
            foreach(DBModels.AppAccess d in dbAppAccesss)
            {
                APIModels.AppAccess a = ToAPIAppAccess(d);
                AppAccessiAppAccesss.Add(a);
            }
            return AppAccessiAppAccesss;
        }
    }
}
