using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class App
    {
        [Key]
        [StringLength(450)]
        public string Id { get; set; }

        [Required]
        [StringLength(450)]
        public string Title { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        [StringLength(450)]
        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public User Owner { get; set; }


    }
}
