using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StartUpApi.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {      
        public virtual ApplicationRole ROLE { get; set; }
    
        public virtual ApplicationUser USER { get; set; }
    }
}
