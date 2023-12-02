using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.API.Models
{
    public class IDFUserRequestModels
    {

        public class Search
        {
            public UsersSearch UsersSearch { get; set; } = new UsersSearch();
            public Paging Paging { get; set; } = new Paging();

            public Search()
            {
                UsersSearch = new UsersSearch();
                Paging = new Paging();
            }
        }


        public class Create
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public bool IsLocked { get; set; }
            public bool IsActive { get; set; }
            public string LastIPAddress { get; set; }
        }

        public class Update
        {
            public string  UserId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public bool IsLocked { get; set; }
            public bool IsActive { get; set; }
        }

        public class Delete
        {
            public string UserId { get; set; }
        }



        public class Details
        {
            public string UserId { get; set; }
        }

        public class ChangePassword
        {
            public string UserId { get; set; }
            public string NewPassword { get; set; }
            public bool ChangePrivateKey { get; set; }

        }

        public class ChangeMyPassword
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
            public bool ChangePrivateKey { get; set; }

        }


        public class Lock
        {
            public string UserId { get; set; }
        }

        public class Unlock
        {
            public string UserId { get; set; }
        }


        public class GetRoles
        {
            public string UserId { get; set; }
        }

        public class GrantRole
        {
            public string UserId { get; set; }
            public string RoleId { get; set; }
        }

        public class RevokeRole
        {
            public string UserId { get; set; }
            public string RoleId { get; set; }
        }



        public class ValidateUserRole
        {
            public string UserId { get; set; }
            public string RoleId { get; set; }
        }

        public class ValidateUserAnyRole
        {
            public string UserId { get; set; }
            public List<string> RoleIds { get; set; }
        }

        public class ValidateTokenRole
        {
            public string Token { get; set; }
            public string RoleId { get; set; }
        }

        public class ValidateTokenAnyRole
        {
            public string Token { get; set; }
            public List<string> RoleIds { get; set; }
        }


    }

}
