using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
            ROLES = new HashSet<ApplicationRole>();
        }

        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public bool IsTemporaryPassword { get; set; }

        public virtual ICollection<ApplicationRole> ROLES { get; set; }
    }
}
