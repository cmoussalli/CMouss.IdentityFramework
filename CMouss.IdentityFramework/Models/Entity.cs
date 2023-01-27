using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class Entity
    {
        [Key]
        [StringLength(450)]
        public string Id { get; set; }

        [Required]
        [StringLength(450)]
        public string Title { get; set; }
    }
}
