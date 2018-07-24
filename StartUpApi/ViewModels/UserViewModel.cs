using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StartUpApi.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            Permissions = new List<PermissionViewModel>();
            Roles = new List<IdNamePair<string>>();
        }
        /// <summary>
        /// Readonly
        /// </summary>
        public string UserID { get; set; }

        [StringLength(128)]
        public string Username { get; set; }
        /// <summary>
        /// User First Name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// User Last Name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Readonly
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Readonly
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Readonly
        /// </summary>
        public DateTime? LastLoginDate { get; set; }
        /// <summary>
        /// Readonly
        /// </summary>
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// Use to determine if the user is using temporary password
        /// </summary>
        public bool IsTemporaryPassword { get; set; }
        /// <summary>
        /// Use only when creating new user
        /// </summary>
        [StringLength(128)]
        public string Password { get; set; }

        /// <summary>
        /// When set to true, an email is send to the user after creation
        /// </summary>
        public bool SendActivationEmail { get; set; }

        public string Email { get; set; }
        /// <summary>
        /// Set if the user is active on the system
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Set if the user has been lockout of the system. Though may be still active
        /// </summary>
        public bool LockoutEnabled { get; set; }
        /// <summary>
        /// Readonly
        /// </summary>
        public string ProfilePictureUrl { get; set; }
       /// <summary>
       /// Readonly. List of user permissions (i.e. distinct collection of all permissions assigned to the user role(s))
       /// </summary>
        public List<PermissionViewModel> Permissions { get; set; }
        /// <summary>
        /// Readonly. List of User roles
        /// </summary>
        public List<IdNamePair<string>> Roles { get; set; }
       
        /// <summary>
        /// Readonly. Determine if the user account has been activated
        /// </summary>
        public bool IsEmailVerified { get; set; }
    }
}
