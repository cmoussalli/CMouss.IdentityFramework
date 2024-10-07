using CMouss.IdentityFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CMouss.IdentityFramework
{
    public partial class UserToken
    {

        public UserClaim ConvertToUserClaim()
        {
            UserClaim userClaim = new UserClaim();
            userClaim.UserId = this.UserId;
            userClaim.UserName = this.User.UserName;
            userClaim.UserFullName = this.User.FullName;
            userClaim.EMail = this.User.Email;

            userClaim.TokenCreateDate = DateTime.UtcNow;
            userClaim.TokenExpireDate = this.ExpireDate;

            userClaim.Roles = new List<string>();
            if (this.User != null)
            {
                userClaim.UserName = this.User.UserName;
                userClaim.UserFullName = this.User.FullName;
                userClaim.EMail = this.User.Email;
                foreach (Role role in this.User.Roles)
                {
                    userClaim.Roles.Add(role.Id);
                }
            }
            return userClaim;
        }


    }
}
