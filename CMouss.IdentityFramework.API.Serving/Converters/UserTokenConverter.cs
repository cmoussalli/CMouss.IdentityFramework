using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class UserTokenConverter
    {
        public static APIModels.UserToken ToAPIUserToken(DBModels.UserToken dbUserToken)
        {
            APIModels.UserToken apiUserToken = new();
            apiUserToken.Token = dbUserToken.Token;
            apiUserToken.ExpireDate = dbUserToken.ExpireDate;
            apiUserToken.IPAddress = dbUserToken.IPAddress;

            APIModels.User apiUser = new();
            if (dbUserToken.User is not null)
            {
                apiUserToken.User = UserConverter.ToAPIUser(dbUserToken.User,false,false,null);
            }

           
            return apiUserToken;
        }
    }
}
