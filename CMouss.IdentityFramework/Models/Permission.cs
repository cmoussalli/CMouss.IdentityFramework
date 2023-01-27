using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class Permission
    {

        [Key]
        [StringLength(450)]
        public string Id { get; set; }


        [Required]
        [StringLength(450)]
        public string EntityId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Entity Entity { get; set; }

        [Required]
        [StringLength(450)]
        public string  PermissionTypeId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual PermissionType PermissionType { get; set; }

        [Required]
        [StringLength(450)]
        public string  RoleId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Role Role { get; set; }

    }
}
