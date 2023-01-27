using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class AppPermissionType
    {
        [Key]
        [StringLength(450)]
        public string Id { get; set; }

        [Required]
        [StringLength(450)]
        public string Title { get; set; }




        [Required]
        [StringLength(450)]
        public string AppId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual App App { get; set; }
    }
}
