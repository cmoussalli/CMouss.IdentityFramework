using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{

    /// <summary>
    /// Set The validation mode to DecryptOnly if you want to validate the token only by decrypting it, or decrypt and validate the token using database
    /// </summary>
    public enum TokenValidationMode
    {
        
        DecryptOnly,
        DecryptAndValidate
    }



    /// <summary>
    /// Use Tenant mode only if you want to validate the authenticated app's both User permission and App Permission
    /// </summary>
    public enum AppPermissionMode
    {
        NotAllowed
        , SimpleMode
        , TenantMode
    }


    public enum SecurityValidationResult
    {
        Ok = 0,
        UnknownError = 1,
        IncorrectParameters = 6,

        IncorrectCredentials = 11,

        IncorrectToken = 21,
        IncorrectAppAccess = 22,
        UnAuthorized = 26,


    }


    public enum IDFAuthenticationMode
    {
        User
    , App
    }
}
