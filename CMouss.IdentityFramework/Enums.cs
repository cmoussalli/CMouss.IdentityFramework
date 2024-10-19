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
        /// <summary>
        /// Use the DefaultTokenValidationMode as per the configuration, this option cannot be setted as the default value in configuration.
        /// </summary>
        UseDefault,

        /// <summary>
        /// Tokens will be validated using decription only, this option will improve the performance since it will skip the validation suing database connection.
        /// </summary>
        DecryptOnly,

        /// <summary>
        /// Tokens will be validated using decription and database validation, this option will require database connection.
        /// </summary>
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
