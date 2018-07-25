using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpApi.Models
{
    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole()
        {
            Id = Guid.NewGuid().ToString();
            ROLEPERMISSIONS = new HashSet<ApplicationRolePermission>();         
        }

        public ApplicationRole(string name)
            : this()
        {
            Name = name;
        }

        /// <summary>
        /// Static Roles cannot be deleted or edited
        /// </summary>
        public bool IsStatic { get; set; }
        /// <summary>
        /// Default role assigned to new users
        /// </summary>
        public bool Default { get; set; }
        [StringLength(200)]
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual ICollection<ApplicationRolePermission> ROLEPERMISSIONS { get; set; }       
    }
}
