using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StartUpApi.Models
{
    public class ApplicationRolePermission
    {
        //[Key]
        //[Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int permissionId { get; set; }
        [ForeignKey("permissionId")]
        public virtual Permission PERMISSION { get; set; }
        //[Key]
        //[Column(Order = 1)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string roleId { get; set; }
        [ForeignKey("roleId")]
        public virtual ApplicationRole ROLE { get; set; }
    }
}
