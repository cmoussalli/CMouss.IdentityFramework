using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class UsersSearchConverter
    {
        public static APIModels.UsersSearch ToAPIUsersSearch(DBModels.UsersSearch dbUsersSearch)
        {
            APIModels.UsersSearch apiUsersSearch = new();
            apiUsersSearch.UserName = dbUsersSearch.UserName;
            apiUsersSearch.Email = dbUsersSearch.Email;
            apiUsersSearch.IsDeleted = dbUsersSearch.IsDeleted;
            apiUsersSearch.IsActive = dbUsersSearch.IsActive;
            apiUsersSearch.IsLocked = dbUsersSearch.IsLocked;
            apiUsersSearch.IsDeleted = dbUsersSearch.IsDeleted;
            apiUsersSearch.FullName = dbUsersSearch.FullName;


            return apiUsersSearch;
        }

        public static DBModels.UsersSearch ToDBUsersSearch(APIModels.UsersSearch apiUsersSearch)
        {
            DBModels.UsersSearch dbUsersSearch = new();
            dbUsersSearch.UserName = apiUsersSearch.UserName;
            dbUsersSearch.Email = apiUsersSearch.Email;
            dbUsersSearch.IsDeleted = apiUsersSearch.IsDeleted;
            dbUsersSearch.IsActive = apiUsersSearch.IsActive;
            dbUsersSearch.IsLocked = apiUsersSearch.IsLocked;
            dbUsersSearch.IsDeleted = apiUsersSearch.IsDeleted;
            dbUsersSearch.FullName = apiUsersSearch.FullName;


            return dbUsersSearch;
        }

    }
}
