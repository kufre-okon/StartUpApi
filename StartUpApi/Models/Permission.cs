using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StartUpApi.Models
{
    public class Permission
    {
        public Permission()
        {
            APPLICATIONROLEPERMISSIONS = new HashSet<ApplicationRolePermission>();
        }

        ///// <summary>
        ///// This is not auto-generated Id, please specify it manuallys
        ///// </summary>
        [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int permissionID { get; set; }
        [StringLength(150)]
        public string name { get; set; }
        public int? parentid { get; set; }
        [ForeignKey("parentid")]
        public virtual Permission PARENTPERMISSION { get; set; }
        public int? level { get; set; }
        [StringLength(200)]
        public string displayname { get; set; }

        public virtual ICollection<ApplicationRolePermission> APPLICATIONROLEPERMISSIONS { get; set; }
    }
}
