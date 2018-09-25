using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        [StringLength(150)]
        public string Name { get; set; }
        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual Permission PARENTPERMISSION { get; set; }
        public int? Level { get; set; }
        [StringLength(200)]
        public string DisplayName { get; set; }

        public virtual ICollection<ApplicationRolePermission> APPLICATIONROLEPERMISSIONS { get; set; }
    }
}
