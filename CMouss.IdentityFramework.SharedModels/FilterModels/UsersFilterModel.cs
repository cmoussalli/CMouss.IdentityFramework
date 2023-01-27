using System;
using System.Collections.Generic;
using System.Text;

namespace CMouss.IdentityFramework.SharedModels
{
    public class UsersFilterModel
    {
        public string? UserName { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public bool? IsLocked { get; set; }

        public bool? IsDeleted { get; set; }

        public bool? IsActive { get; set; }

      
    }
}
