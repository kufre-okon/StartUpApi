using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class OrganizationUnit
    {
        public OrganizationUnit()
        {
            USERORGANIZATION = new HashSet<UserOrganization>();
        }

        [Key]
        public int OrganizationId { get; set; }
        [StringLength(70)]
        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual OrganizationUnit PARENT { get; set; }

        public int? Level { get; set; }

        public virtual ICollection<UserOrganization> USERORGANIZATION { get; set; }
    }
}
