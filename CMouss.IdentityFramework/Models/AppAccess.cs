using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class AppAccess
    {
        [Key]
        [StringLength(450)]
        public string Id { get; set; }

        [Required]
        [StringLength(500)]
        public string AccessKey { get; set; }

        [Required]
        [StringLength(500)]
        public string AccessSecret { get; set; }

        public DateTime ExpireDate { get; set; }

        [StringLength(450)]
        public string UserId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public User User { get; set; }

        [Required]
        [StringLength(450)]
        public string AppId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public App App { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual List<AppAccessPermission> AppAccessPermissions { get; set; } = new List<AppAccessPermission>();
    }
}
