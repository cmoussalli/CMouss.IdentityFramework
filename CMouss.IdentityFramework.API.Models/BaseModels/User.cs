using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.API.Models
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        //public string Password { get; set; }
        //public string PrivateKey { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsLocked { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Role> Roles { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<App> Apps { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Permission> Permissions { get; set; }
    }
}
