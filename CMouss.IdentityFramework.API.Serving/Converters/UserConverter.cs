using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class UserConverter
    {
        public static APIModels.User ToAPIUser(DBModels.User dbUser, bool loadApps, bool loadRoles,List<DBModels.Permission>? dbPermissions)
        {
            APIModels.User apiUser = new();
            apiUser.Id = dbUser.Id;
            apiUser.UserName = dbUser.UserName;
            apiUser.FullName = dbUser.FullName;
            apiUser.Email = dbUser.Email;
            apiUser.IsLocked = dbUser.IsLocked;
            apiUser.IsActive = dbUser.IsActive;
            apiUser.CreateDate = dbUser.CreateDate;
            apiUser.LastIPAddress = dbUser.LastIPAddress;

            if (loadApps) { apiUser.Apps = AppConverter.ToAPIAppsList(dbUser.Apps); }
            if (loadRoles) { apiUser.Roles = RoleConverter.ToAPIRolesList(dbUser.Roles,false,false); }
            if (dbPermissions != null) { apiUser.Permissions = PermissionConverter.ToAPIPermissionsList(dbPermissions); }
            return apiUser;
        }

        public static List<APIModels.User> ToAPIUsersList(List<DBModels.User> dbUsers)
        {
            List<APIModels.User> apiUsers = new();
            foreach (DBModels.User d in dbUsers)
            {
                APIModels.User o = ToAPIUser(d,false,false,null);
                apiUsers.Add(o);
            }
            return apiUsers;
        }
    }
}
