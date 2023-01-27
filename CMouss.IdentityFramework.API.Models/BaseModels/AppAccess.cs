using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.API.Models
{
    public class AppAccess
    {
        public string Id { get; set; }
        public string AccessKey { get; set; }
        public string AccessSecret { get; set; }
        public DateTime ExpireDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public User User { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public App App { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<AppAccessPermission> AppAccessPermissions { get; set; } = new();
    }
}
