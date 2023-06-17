using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class AuthResult
    {

        public SecurityValidationResult SecurityValidationResult { get; set; }

        public IDFAuthenticationMode AuthenticationMode { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UserToken UserToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AppAccess AppAccess { get; set; }

        public AuthResult()
        {
        }

        public AuthResult(UserToken userToken)
        {
            SecurityValidationResult = SecurityValidationResult.Ok;
            AuthenticationMode = IDFAuthenticationMode.User;
            UserToken = userToken;
        }

        public AuthResult(AppAccess appAccess)
        {
            SecurityValidationResult = SecurityValidationResult.Ok;
            AuthenticationMode = IDFAuthenticationMode.App;
            AppAccess = appAccess;
        }


    }


}
