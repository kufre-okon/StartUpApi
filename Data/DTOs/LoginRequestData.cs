using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Data.DTO
{
    public class LoginRequestData
    {

        /// <summary>
        /// Application clientId
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        [StringLength(128)]
        [Required]
        public string Username { get; set; }
       
        [StringLength(128)]
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Required for native application
        /// </summary>
        public string ClientSecret { get; set; }
    }

    public class LoginResponseData
    {       
        public string FullName { get; set; }       
        public string ProfilePictureUrl { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }
        public DateTime TokenIssued { get; set; }

        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
        public List<IdNamePair<string>> Roles { get; set; } = new List<IdNamePair<string>>();
        public string Username { get;  set; }
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
    }

    public class RefreshTokenResponseData
    {  
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }        
        public string RefreshToken { get; set; }
        public DateTime TokenIssued { get;  set; }
    }
    public class RefreshTokenRequestData
    {
        public string ClientSecret { get; set; }
        public string ClientId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }    
}

