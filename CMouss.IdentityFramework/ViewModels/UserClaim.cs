using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.ViewModels
{
    public class UserClaim
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }

        public string EMail { get; set; }

        public DateTime TokenCreateDate { get; set; }
        public DateTime TokenExpireDate { get; set; }

        public string IPAddress { get; set; }
        public List<string> Roles { get; set; }




        public AuthResult ToAuthResult()
        {
            AuthResult result = new();
            result.SecurityValidationResult = SecurityValidationResult.Ok;
            result.AuthenticationMode = IDFAuthenticationMode.User;
            
            result.UserToken = new();
            result.UserToken.UserId = UserId;
            result.UserToken.Token = Helpers.Encrypt( JsonSerializer.Serialize(this) ,IDFManager.TokenEncryptionKey);
            result.UserToken.IPAddress = IPAddress;
            result.UserToken.ExpireDate = TokenExpireDate;

            result.UserToken.User = new();
            result.UserToken.User.Id = UserId;
            result.UserToken.User.FullName = UserFullName;
            result.UserToken.User.IsActive = true;
            result.UserToken.User.Email = EMail;
            result.UserToken.User.UserName = UserName;
            result.UserToken.User.Roles = Helpers.GetUserClaimRoles(this);


            return result;
        }


    }
}
