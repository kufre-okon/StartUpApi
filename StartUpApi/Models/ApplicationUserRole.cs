using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StartUpApi.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {       
        [ForeignKey("RoleId")]
        public virtual ApplicationRole ROLE { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser USER { get; set; }
    }
}
