using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StartUpApi.Data.Models
{
    public class UserOrganization
    {
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual OrganizationUnit ORGANIZATIONUNIT { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser USER { get; set; }
    }
}
