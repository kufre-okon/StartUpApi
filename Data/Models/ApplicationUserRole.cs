using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {       
       [ForeignKey("RoleId")]
        public virtual ApplicationRole ROLE { get; set; }
       [ForeignKey("UserId")]
        public virtual ApplicationUser USER { get; set; }
    }
}
