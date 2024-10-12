using CMouss.IdentityFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.Converters
{
    public class UserTokenConverter
    {

        public UserToken FromUserClaim(UserClaim claim)
        {
            return new UserToken
            {
                ExpireDate = claim.TokenExpireDate,
                UserId = claim.UserId,
                User = new User
                {
                    Id = claim.UserId,
                    UserName = claim.UserName,
                    FullName = claim.UserFullName,
                    Email = claim.EMail,
                    Roles = claim.Roles.Select(o => Storage.Roles.FirstOrDefault(r => r.Id == o)).ToList()
                },


            };
        }


    }
}
