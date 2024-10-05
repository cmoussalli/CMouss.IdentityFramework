using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<string> Roles { get; set; }



    }
}
