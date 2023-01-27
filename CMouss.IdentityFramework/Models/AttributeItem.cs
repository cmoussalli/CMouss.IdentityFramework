using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class AttributeItem
    {
        [Key]
        [StringLength(450)]
        public string Id { get; set; }


        private List<string> values = new List<string>();
        [NotMapped]
        public List<string> Values { get; set; } = new List<string>();

        public string ValuesString
        {
            get
            {
                return String.Join(",,", Values);
            }
            set
            {
                Values = value.Split(",,").ToList();
            }
        }


        [Required]
        [StringLength(450)]
        public string AttributeTypeId { get; set; }
        public virtual AttributeType AttributeType { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }
        public virtual User User { get; set; }

    }
}
