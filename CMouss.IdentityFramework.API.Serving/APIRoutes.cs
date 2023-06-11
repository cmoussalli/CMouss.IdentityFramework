using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.API.Serving
{
    public class APIRoutes
    {
        #region Base
        public const string Base = "api/Identity";

        #endregion


        #region Test
        public static class Test
        {
            public const string TestMain = Base + "/Test";
            const String entity = "Test";

            public const string Echo = Base + "/" + entity + "/Echo";
        }
        #endregion


        #region Auth
        public static class Auth
        {
            const String entity = "Auth";
            
            public const string UserLogin = Base + "/" + entity + "/UserLogin";
            public const string UserToken = Base + "/" + entity + "/UserToken";


            public const string AppLogin = Base + "/" + entity + "/AppLogin";
            public const string AppAccess = Base + "/" + entity + "/AppAccess";


        }
        #endregion


        #region User
        public static class User
        {
            const String entity = "User";
            public const string Register = Base + "/" + entity + "/Register";
            public const string Login = Base + "/" + entity + "/Login";
            public const string Logout = Base + "/" + entity + "/Logout";
            public const string ValidateToken = Base + "/" + entity + "/ValidateToken";
            public const string ChangePassword = Base + "/" + entity + "/ChangePassword";
            public const string ChangeMyPassword = Base + "/" + entity + "/ChangeMyPassword";

            public const string Search = Base + "/" + entity + "/Search";
            public const string Details = Base + "/" + entity + "/Details";
            public const string Create = Base + "/" + entity + "/Create";
            public const string Update = Base + "/" + entity + "/Update";
            public const string Delete = Base + "/" + entity + "/Delete";
            public const string Enable = Base + "/" + entity + "/Enable";
            public const string Disable = Base + "/" + entity + "/Disable";

            public const string Lock = Base + "/" + entity + "/Lock";
            public const string Unlock = Base + "/" + entity + "/Unlock";

            public const string GetRoles = Base + "/" + entity + "/GetRoles";
            public const string GrantRole = Base + "/" + entity + "/GrantRole";
            public const string RevokeRole = Base + "/" + entity + "/RevokeRole";

            public const string ValidateUserRole = Base + "/" + entity + "/ValidateUserRole";
            public const string ValidateUserAnyRole = Base + "/" + entity + "/ValidateUserAnyRole";

            public const string ValidateTokenRole = Base + "/" + entity + "/ValidateTokenRole";
            public const string ValidateTokenAnyRole = Base + "/" + entity + "/ValidateTokenAnyRole";

        }
        #endregion

        #region App
        public static class App
        {
            const string entity = "App";
            public const string Search = Base + "/" + entity + "/Search";
            public const string Details = Base + "/" + entity + "/Details";
            public const string GetAll = Base + "/" + entity + "/GetAll";
            public const string Create = Base + "/" + entity + "/Create";
            public const string Update = Base + "/" + entity + "/Update";
            public const string Delete = Base + "/" + entity + "/Delete";

            public const string GetUserApps = Base + "/" + entity + "/GetUserApps";
        }
        #endregion

        #region AppAccess
        public static class AppAccess
        {
            const string entity = "AppAccess";
            public const string GetAll = Base + "/" + entity + "/GetAll";
            public const string Generate = Base + "/" + entity + "/Generate";
            public const string Validate = Base + "/" + entity + "/Validate";
            public const string Delete = Base + "/" + entity + "/Delete";
            public const string Refresh = Base + "/" + entity + "/RefreshAccessSecret";
            public const string Clean = Base + "/" + entity + "/CleanExpiredAppAccesss";
        }
        #endregion



    }
}
