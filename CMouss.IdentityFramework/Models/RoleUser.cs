using Microsoft.EntityFrameworkCore;
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

    [Table("RoleUser")]
    [PrimaryKey(nameof(RoleId), nameof(UserId))]
    public class RoleUser
    {
        //[Key]
        //public long Id { get; set; }

        [StringLength(450)]
        [ForeignKey(nameof(Role))]
        public string RoleId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Role Role { get; set; }

        [StringLength(450)]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual User User { get; set; }
    }
}
