using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace StartUpApi.Data.Models
{
    public class ApplicationUser : IdentityUser<string>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
            USERROLES = new HashSet<ApplicationUserRole>();
            USERORGANIZATION = new HashSet<UserOrganization>();
        }

        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public bool IsTemporaryPassword { get; set; }
        public string ProfilePictureUrl { get; set; }

        /*
         * Navigation property for the roles this user belongs to.
         */
        public virtual ICollection<ApplicationUserRole> USERROLES { get; set; }
        public virtual ICollection<UserOrganization> USERORGANIZATION { get; set; }
    }
}
