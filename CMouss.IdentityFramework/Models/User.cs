using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CMouss.IdentityFramework
{
    public class User
    {
        [Key]
        [StringLength(450)]
        public string Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserName { get; set; }

        [Required]
        [StringLength(128)]
        public string Password { get; set; }

        [Required]
        [StringLength(450)]
        public string PrivateKey { get; set; }

        public string FullName { get; set; }

        [Required]
        [StringLength(128)]
        public string Email { get; set; }

        [Required]
        public bool IsLocked { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public DateTimeOffset CreateDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual List<Role> Roles { get; set; } = new List<Role>();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual List<App> Apps { get; set; }
    }
}
