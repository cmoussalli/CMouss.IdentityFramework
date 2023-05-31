using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.API.Models
{
    public class IDFUserResponseModels
    {

        public class Login : GenericResponseModel
        {
            public UserToken UserToken { get; set; }
        }

        public class Search : GenericResponseModel
        {
            public List<User> Users { get; set; }
        }

        public class Details : GenericResponseModel
        {
            public User User { get; set; }
        }

        public class GetRoles : GenericResponseModel
        {
            public List<Role> Roles { get; set; }
        }




    }
}
