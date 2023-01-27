using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class AppAccessPermission
    {
        [Key]
        [StringLength(450)]
        public string Id { get; set; }

        //[Required]
        [StringLength(450)]
        public string? AppAccessId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual AppAccess AppAccess { get; set; }

        [Required]
        [StringLength(450)]
        public string AppPermissionTypeId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual AppPermissionType AppPermissionType { get; set; }





    }
}
