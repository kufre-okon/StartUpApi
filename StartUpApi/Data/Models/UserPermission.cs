using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StartUpApi.Data.Models
{
    public class UserPermission
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
        public string userId { get; set; }
        [ForeignKey("userId")]
        public virtual ApplicationUser USER { get; set; }
    }
}
