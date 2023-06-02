using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class AuthResultConverter
    {
        public static APIModels.AuthResult ToAPIAuthResult(DBModels.AuthResult dbAuthResult)
        {
            APIModels.AuthResult result = new APIModels.AuthResult();
            result.SecurityValidationResult = dbAuthResult.SecurityValidationResult.ToString();
            result.AuthenticationMode = dbAuthResult.AuthenticationMode.ToString();

            if (dbAuthResult.UserToken is not null)
            { result.UserToken = Converters.UserTokenConverter.ToAPIUserToken(dbAuthResult.UserToken); }

            if (dbAuthResult.AppAccess is not null)
            { result.AppAccess = Converters.AppAccessConverter.ToAPIAppAccess(dbAuthResult.AppAccess); }

            return result;
        }

    }
}
